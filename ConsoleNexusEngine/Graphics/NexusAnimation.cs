using System; // Added for EventArgs and Action
using System.Collections.Generic; // Added for List
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;
using System.Runtime.InteropServices; // Added for PropertyItem access (optional but good practice)
using System.Threading; // Added for CancellationToken
using System.Threading.Tasks; // Added for Task

namespace ConsoleNexusEngine.Graphics
{
    /// <summary>
    /// Represents the playback state of the animation.
    /// </summary>
    public enum AnimationState
    {
        /// <summary>
        /// The animation is stopped and reset to the beginning.
        /// </summary>
        Stopped,
        /// <summary>
        /// The animation is currently playing.
        /// </summary>
        Playing,
        /// <summary>
        /// The animation is paused at the current frame.
        /// </summary>
        Paused
    }

    /// <summary>
    /// Represents the playback direction of the animation.
    /// </summary>
    public enum PlaybackDirection
    {
        /// <summary>
        /// Plays the animation frames in forward order.
        /// </summary>
        Forward,
        /// <summary>
        /// Plays the animation frames in reverse order.
        /// </summary>
        Backward,
        /// <summary>
        /// Plays the animation forward then backward, repeating.
        /// </summary>
        PingPong
    }

    /// <summary>
    /// Represents an animation, typically loaded from a GIF, that can be controlled and displayed in the console.
    /// </summary>
    public sealed class NexusAnimation : IDisposable // Added IDisposable
    {
        private const int PropertyTagFrameDelay = 0x5100;
        private static readonly TimeSpan DefaultFrameDelay = TimeSpan.FromMilliseconds(100); // Default if GIF doesn't specify

        private readonly ReadOnlyMemory<NexusImage> _frames;
        private readonly ReadOnlyMemory<TimeSpan> _frameDelays; // Store individual frame delays

        private int _currentFrameIndex;
        private TimeSpan _elapsedTime;
        private AnimationState _state;
        private PlaybackDirection _currentPingPongDirection = PlaybackDirection.Forward; // For PingPong mode
        private int _loopsCompleted = 0;

        private HttpClient? _httpClient; // For managing HttpClient lifetime
        private bool _isHttpClientOwned = false; // Track if we created the client

        /// <summary>
        /// Gets the total number of frames in the animation.
        /// </summary>
        public int FrameCount => _frames.Length;

        /// <summary>
        /// Gets the dimensions (width and height) of the animation frames.
        /// Assumes all frames have the same size.
        /// </summary>
        public NexusSize Size { get; }

        /// <summary>
        /// Gets the duration of a single loop of the animation.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Gets or sets the current playback state of the animation.
        /// </summary>
        public AnimationState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    if (_state == AnimationState.Stopped)
                    {
                        Reset();
                    }
                    StateChanged?.Invoke(this, _state);
                }
            }
        }

        /// <summary>
        /// Gets the index of the currently displayed frame.
        /// </summary>
        public int CurrentFrameIndex => _currentFrameIndex;

        /// <summary>
        /// Gets the currently displayed frame.
        /// </summary>
        public NexusImage CurrentFrame => _frames.Span[_currentFrameIndex];

        /// <summary>
        /// Gets or sets whether the animation should loop indefinitely.
        /// Defaults to true. If set to false, LoopCount determines behavior.
        /// </summary>
        public bool IsLooping { get; set; } = true;

        /// <summary>
        /// Gets or sets the number of times the animation should loop.
        /// Only effective if IsLooping is false.
        /// A value of 0 or 1 means play once. A value of 2 means play twice, etc.
        /// Negative values are ignored (treated as 1).
        /// </summary>
        public int LoopCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets the playback direction.
        /// </summary>
        public PlaybackDirection Direction { get; set; } = PlaybackDirection.Forward;

        /// <summary>
        /// Gets or sets a speed multiplier for playback.
        /// 1.0 is normal speed, 0.5 is half speed, 2.0 is double speed.
        /// Must be greater than 0.
        /// </summary>
        public double SpeedMultiplier { get; set; } = 1.0;

        /// <summary>
        /// Event triggered when the animation finishes playing (or completes a loop cycle if not looping indefinitely).
        /// </summary>
        public event EventHandler? AnimationFinished;

        /// <summary>
        /// Event triggered when the animation loops.
        /// </summary>
        public event EventHandler? AnimationLooped;

        /// <summary>
        /// Event triggered when the current frame index changes.
        /// The integer argument is the new frame index.
        /// </summary>
        public event Action<int>? FrameChanged;

        /// <summary>
        /// Event triggered when the animation state (Playing, Paused, Stopped) changes.
        /// </summary>
        public event Action<NexusAnimation, AnimationState>? StateChanged;


        // --- Private Constructor (Core Logic) ---

        private NexusAnimation(Bitmap sourceBitmap, NexusColorProcessor imageProcessor, in NexusSize? targetSize, bool disposeBitmap)
        {
            if (sourceBitmap.RawFormat.Guid != ImageFormat.Gif.Guid)
            {
                if (disposeBitmap) sourceBitmap.Dispose();
                throw new ArgumentException("The source image must be a GIF.", nameof(sourceBitmap));
            }

            try
            {
                var frameData = ExtractFrames(sourceBitmap, imageProcessor, targetSize);
                _frames = frameData.Frames;
                _frameDelays = frameData.Delays;
                Duration = frameData.TotalDuration;

                if (_frames.Length == 0)
                {
                    throw new ArgumentException("GIF contains no frames.", nameof(sourceBitmap));
                }

                Size = _frames.Span[0].Size; // Assuming all frames are the same size after processing
                Reset(); // Initialize state
            }
            finally
            {
                // Dispose the source bitmap if we created it implicitly (e.g., from file path)
                if (disposeBitmap)
                {
                    sourceBitmap.Dispose();
                }
            }
        }

        // --- Constructors ---

        /// <summary>
        /// Initializes a new NexusAnimation from a GIF file.
        /// </summary>
        /// <param name="filepath">The path to the GIF file.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        public NexusAnimation(string filepath, NexusColorProcessor imageProcessor)
            : this(new Bitmap(filepath), imageProcessor, null, true) // Dispose the loaded bitmap
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a Bitmap object (must be a GIF).
        /// The caller is responsible for disposing the provided Bitmap if disposeBitmap is false.
        /// </summary>
        /// <param name="animationBitmap">The source GIF Bitmap.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="disposeBitmap">Whether this constructor should dispose the animationBitmap upon completion (defaults to false).</param>
        public NexusAnimation(Bitmap animationBitmap, NexusColorProcessor imageProcessor, bool disposeBitmap = false)
            : this(animationBitmap, imageProcessor, null, disposeBitmap)
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a GIF file, resizing it to a percentage of its original size.
        /// </summary>
        /// <param name="filepath">The path to the GIF file.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="percentage">The desired percentage size (e.g., 0.5f for 50%).</param>
        public NexusAnimation(string filepath, NexusColorProcessor imageProcessor, in float percentage)
            : this(new Bitmap(filepath), imageProcessor, percentage, true) // Dispose the loaded bitmap
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a Bitmap object (must be a GIF), resizing it to a percentage.
        /// The caller is responsible for disposing the provided Bitmap if disposeBitmap is false.
        /// </summary>
        /// <param name="animationBitmap">The source GIF Bitmap.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="percentage">The desired percentage size (e.g., 0.5f for 50%).</param>
        /// <param name="disposeBitmap">Whether this constructor should dispose the animationBitmap upon completion (defaults to false).</param>
        public NexusAnimation(Bitmap animationBitmap, NexusColorProcessor imageProcessor, in float percentage, bool disposeBitmap = false)
            : this(animationBitmap, imageProcessor, ImageHelper.GetSize(animationBitmap.Width, animationBitmap.Height, percentage), disposeBitmap)
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a GIF file, resizing it to a specific size.
        /// </summary>
        /// <param name="filepath">The path to the GIF file.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="size">The desired size.</param>
        public NexusAnimation(string filepath, NexusColorProcessor imageProcessor, in NexusSize size)
            : this(new Bitmap(filepath), imageProcessor, size, true) // Dispose the loaded bitmap
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a Bitmap object (must be a GIF), resizing it to a specific size.
        /// The caller is responsible for disposing the provided Bitmap if disposeBitmap is false.
        /// </summary>
        /// <param name="animationBitmap">The source GIF Bitmap.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="size">The desired size.</param>
        /// <param name="disposeBitmap">Whether this constructor should dispose the animationBitmap upon completion (defaults to false).</param>
        public NexusAnimation(Bitmap animationBitmap, NexusColorProcessor imageProcessor, in NexusSize size, bool disposeBitmap = false)
            : this(animationBitmap, imageProcessor, new NexusSize?(size), disposeBitmap)
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a sequence of pre-defined NexusImage frames.
        /// Assumes a default frame delay.
        /// </summary>
        /// <param name="frames">The images for the animation.</param>
        /// <exception cref="ArgumentException">Thrown if no frames are provided.</exception>
        public NexusAnimation(params NexusImage[] frames)
            : this(frames, Enumerable.Repeat(DefaultFrameDelay, frames.Length).ToArray())
        { }

        /// <summary>
        /// Initializes a new NexusAnimation from a sequence of pre-defined NexusImage frames and their corresponding delays.
        /// </summary>
        /// <param name="frames">The images for the animation.</param>
        /// <param name="frameDelays">The display duration for each frame.</param>
        /// <exception cref="ArgumentException">Thrown if no frames are provided or if frame/delay counts don't match.</exception>
        public NexusAnimation(IReadOnlyList<NexusImage> frames, IReadOnlyList<TimeSpan> frameDelays)
        {
            if (frames == null || frames.Count == 0)
                throw new ArgumentException("At least one frame must be provided.", nameof(frames));
            if (frameDelays == null || frameDelays.Count != frames.Count)
                throw new ArgumentException("The number of frames and frame delays must match.", nameof(frameDelays));

            _frames = new ReadOnlyMemory<NexusImage>(frames is NexusImage[] arr ? arr : frames.ToArray()); // Optimize if already array
            _frameDelays = new ReadOnlyMemory<TimeSpan>(frameDelays is TimeSpan[] tsArr ? tsArr : frameDelays.ToArray());

            Size = _frames.Span[0].Size; // Assume consistent size
            Duration = TimeSpan.Zero;
            foreach (var delay in _frameDelays.Span)
            {
                Duration += delay;
            }
            Reset();
        }

        // --- Static Factory Methods ---

        /// <summary>
        /// Asynchronously creates a NexusAnimation from a GIF URL.
        /// </summary>
        /// <param name="url">The URL of the GIF file.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="targetSize">Optional desired size of the animation.</param>
        /// <param name="httpClient">Optional HttpClient instance to use.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A Task representing the asynchronous operation, yielding the NexusAnimation.</returns>
        /// <exception cref="HttpRequestException">Thrown if the download fails.</exception>
        /// <exception cref="ArgumentException">Thrown if the downloaded content is not a valid GIF.</exception>
        public static async Task<NexusAnimation> FromUrlAsync(
            Uri url,
            NexusColorProcessor imageProcessor,
            NexusSize? targetSize = null,
            HttpClient? httpClient = null,
            CancellationToken cancellationToken = default)
        {
            bool ownClient = httpClient == null;
            var client = httpClient ?? new HttpClient();

            try
            {
                using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode(); // Throws HttpRequestException on failure

                using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                // Load into memory stream first to allow seeking, required by Bitmap(stream)
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream, 81920, cancellationToken).ConfigureAwait(false);
                memoryStream.Position = 0; // Reset stream position

                // Bitmap constructor might throw ArgumentException if stream is not valid image
                var bitmap = new Bitmap(memoryStream);

                // We pass disposeBitmap: true because we created this bitmap
                var animation = new NexusAnimation(bitmap, imageProcessor, targetSize, true);

                // If we created the client, assign it to the animation for later disposal
                if (ownClient)
                {
                    animation._httpClient = client;
                    animation._isHttpClientOwned = true;
                    client = null; // Prevent disposal in finally block if ownership transferred
                }
                return animation;
            }
            finally
            {
                // Dispose the client only if we created it AND ownership wasn't transferred
                if (ownClient && client != null)
                {
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Synchronously creates a NexusAnimation from a GIF URL.
        /// Use FromUrlAsync for non-blocking operations.
        /// </summary>
        /// <param name="url">The URL of the GIF file.</param>
        /// <param name="imageProcessor">The image processor to use.</param>
        /// <param name="targetSize">Optional desired size of the animation.</param>
        /// <param name="httpClient">Optional HttpClient instance to use.</param>
        /// <returns>The created NexusAnimation.</returns>
        public static NexusAnimation FromUrl(Uri url, NexusColorProcessor imageProcessor, NexusSize? targetSize = null, HttpClient? httpClient = null)
        {
            // Basic synchronous wrapper - consider using a dedicated sync-over-async library
            // or encouraging async usage if performance/responsiveness is critical.
            try
            {
                // Note: .Result can cause deadlocks in some contexts (e.g., UI threads).
                // Use with caution or refactor calling code to be async.
                return FromUrlAsync(url, imageProcessor, targetSize, httpClient).GetAwaiter().GetResult();
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Failed to download or process animation from {url}. Status code: {ex.StatusCode}", ex);
            }
            catch (ArgumentException ex)
            {
                 throw new ArgumentException($"The content from {url} is not a valid GIF image.", ex);
            }
            catch(Exception ex) // Catch broader exceptions during sync wait
            {
                throw new Exception($"An unexpected error occurred while fetching animation from {url}.", ex);
            }
        }


        // --- Playback Control Methods ---

        /// <summary>
        /// Starts playing the animation from the current position.
        /// If stopped, starts from the beginning.
        /// </summary>
        public void Play()
        {
            if (State == AnimationState.Playing) return;

            if (State == AnimationState.Stopped)
            {
                 // Reset timing and loop count if restarting from stopped state
                 _elapsedTime = TimeSpan.Zero;
                 _loopsCompleted = 0;
                 // Set initial frame based on direction
                 _currentFrameIndex = (Direction == PlaybackDirection.Backward) ? FrameCount - 1 : 0;
                 _currentPingPongDirection = PlaybackDirection.Forward; // Reset ping-pong direction
                 FrameChanged?.Invoke(_currentFrameIndex);
            }
            State = AnimationState.Playing;
        }

        /// <summary>
        /// Pauses the animation at the current frame.
        /// </summary>
        public void Pause()
        {
            if (State == AnimationState.Playing)
            {
                State = AnimationState.Paused;
            }
        }

        /// <summary>
        /// Stops playback and resets the animation to the beginning.
        /// </summary>
        public void Stop()
        {
            State = AnimationState.Stopped; // Setter handles Reset()
        }

        /// <summary>
        /// Resets the animation to its initial state (first frame, stopped).
        /// </summary>
        public void Reset()
        {
            _state = AnimationState.Stopped; // Set directly to avoid loop in State setter
            _currentFrameIndex = (Direction == PlaybackDirection.Backward && FrameCount > 0) ? FrameCount - 1 : 0;
            _elapsedTime = TimeSpan.Zero;
            _loopsCompleted = 0;
            _currentPingPongDirection = PlaybackDirection.Forward;
            // Consider if FrameChanged should be invoked on Reset. Usually not.
            // StateChanged will be invoked by the caller setting State = Stopped.
        }

        /// <summary>
        /// Jumps to a specific frame index.
        /// </summary>
        /// <param name="frameIndex">The index of the frame to display.</param>
        /// <param name="pauseAfterJump">If true, the animation state will be set to Paused after jumping.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if frameIndex is invalid.</exception>
        public void GoToFrame(int frameIndex, bool pauseAfterJump = true)
        {
            if (frameIndex < 0 || frameIndex >= FrameCount)
            {
                throw new ArgumentOutOfRangeException(nameof(frameIndex), $"Frame index must be between 0 and {FrameCount - 1}.");
            }

            if (_currentFrameIndex != frameIndex)
            {
                _currentFrameIndex = frameIndex;
                _elapsedTime = TimeSpan.Zero; // Reset time elapsed for the new frame
                FrameChanged?.Invoke(_currentFrameIndex);
            }

            if (pauseAfterJump)
            {
                State = AnimationState.Paused;
            }
        }

        /// <summary>
        /// Updates the animation state based on the elapsed time.
        /// Call this method repeatedly in your game loop or timer callback.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update.</param>
        /// <returns>The NexusImage frame to display for the current state.</returns>
        public NexusImage Update(TimeSpan deltaTime)
        {
            if (State != AnimationState.Playing || FrameCount <= 1 || SpeedMultiplier <= 0)
            {
                // Return current frame if not playing, only one frame, or invalid speed
                return CurrentFrame;
            }

            // Adjust delta time by speed multiplier
            _elapsedTime += TimeSpan.FromTicks((long)(deltaTime.Ticks * SpeedMultiplier));

            // Get the delay for the *current* frame
            TimeSpan currentFrameDelay = _frameDelays.Span[_currentFrameIndex];

            // If the delay is zero or negative, treat it as a default small delay to prevent infinite loops
             if (currentFrameDelay <= TimeSpan.Zero)
             {
                 currentFrameDelay = DefaultFrameDelay; // Or perhaps TimeSpan.FromMilliseconds(10)
             }


            // Process frame changes based on elapsed time
            while (_elapsedTime >= currentFrameDelay)
            {
                _elapsedTime -= currentFrameDelay;
                int previousFrameIndex = _currentFrameIndex;

                // Advance frame logic
                if (!AdvanceFrame())
                {
                    // Animation finished and shouldn't loop/continue
                    State = AnimationState.Stopped;
                    AnimationFinished?.Invoke(this, EventArgs.Empty);
                    // Ensure we return the last frame after stopping
                     _currentFrameIndex = ClampFrameIndex(previousFrameIndex); // Stay on the last valid frame
                    return _frames.Span[_currentFrameIndex];
                }

                 // Update delay for the *new* frame for the next iteration (if any)
                 currentFrameDelay = _frameDelays.Span[_currentFrameIndex];
                 if (currentFrameDelay <= TimeSpan.Zero)
                 {
                     currentFrameDelay = DefaultFrameDelay;
                 }

                // Fire event only if the frame actually changed
                if (_currentFrameIndex != previousFrameIndex)
                {
                    FrameChanged?.Invoke(_currentFrameIndex);
                }

                // If state changed during AdvanceFrame (e.g., stopped), exit loop
                if (State != AnimationState.Playing) break;
            }

            return CurrentFrame;
        }

        // --- Helper Methods ---

        /// <summary>
        /// Advances the frame index based on direction, looping, and ping-pong mode.
        /// </summary>
        /// <returns>True if the animation should continue playing, False if it should stop.</returns>
        private bool AdvanceFrame()
        {
            int nextFrameIndex = _currentFrameIndex;
            bool looped = false;

            PlaybackDirection effectiveDirection = (Direction == PlaybackDirection.PingPong)
                ? _currentPingPongDirection
                : Direction;

            if (effectiveDirection == PlaybackDirection.Forward)
            {
                nextFrameIndex++;
                if (nextFrameIndex >= FrameCount) // Reached the end
                {
                    if (Direction == PlaybackDirection.PingPong)
                    {
                        _currentPingPongDirection = PlaybackDirection.Backward;
                        // Move back to the second to last frame immediately
                        nextFrameIndex = Math.Max(0, FrameCount - 2);
                        looped = true; // Consider ping-pong switch a loop
                    }
                    else // Forward direction, end reached
                    {
                        _loopsCompleted++;
                        looped = true;
                        if (IsLooping || _loopsCompleted < Math.Max(1, LoopCount))
                        {
                            nextFrameIndex = 0; // Loop back to start
                        }
                        else
                        {
                            return false; // Stop playing
                        }
                    }
                }
            }
            else // Backward or PingPong-Backward
            {
                nextFrameIndex--;
                if (nextFrameIndex < 0) // Reached the beginning
                {
                     if (Direction == PlaybackDirection.PingPong)
                    {
                        _currentPingPongDirection = PlaybackDirection.Forward;
                        // Move forward to the second frame immediately
                        nextFrameIndex = Math.Min(1, FrameCount - 1);
                         // Don't count backward->forward ping-pong switch as a full loop completion? Or maybe it should? Let's count it.
                        _loopsCompleted++;
                         looped = true;
                         if (!IsLooping && _loopsCompleted >= Math.Max(1, LoopCount))
                         {
                             return false; // Stop playing if loop count reached
                         }
                    }
                     else // Backward direction, beginning reached
                     {
                        _loopsCompleted++;
                        looped = true;
                        if (IsLooping || _loopsCompleted < Math.Max(1, LoopCount))
                        {
                             nextFrameIndex = FrameCount - 1; // Loop back to end
                        }
                        else
                        {
                             return false; // Stop playing
                        }
                     }
                }
            }

            _currentFrameIndex = ClampFrameIndex(nextFrameIndex);

            if (looped)
            {
                 AnimationLooped?.Invoke(this, EventArgs.Empty);
            }

            return true; // Continue playing
        }

        private int ClampFrameIndex(int index)
        {
             if (FrameCount == 0) return 0;
             return Math.Max(0, Math.Min(index, FrameCount - 1));
        }


        private static (ReadOnlyMemory<NexusImage> Frames, ReadOnlyMemory<TimeSpan> Delays, TimeSpan TotalDuration) ExtractFrames(
            Bitmap bitmap, NexusColorProcessor processor, in NexusSize? targetSize)
        {
            var frameCount = 0;
            try
            {
                frameCount = bitmap.GetFrameCount(FrameDimension.Time);
            }
            catch (Exception ex) // Catch potential errors if format doesn't support time dimension
            {
                 throw new ArgumentException("Could not get frame count. Ensure the source is an animated GIF.", ex);
            }

            if (frameCount <= 0)
            {
                return (ReadOnlyMemory<NexusImage>.Empty, ReadOnlyMemory<TimeSpan>.Empty, TimeSpan.Zero);
            }

            var images = new NexusImage[frameCount];
            var delays = new TimeSpan[frameCount];
            TimeSpan totalDuration = TimeSpan.Zero;

            // Frame delay is stored in PropertyTagFrameDelay.
            // It's an array of longs (UInt32), each representing delay in 1/100th seconds.
            PropertyItem? frameDelayItem = null;
            try
            {
                frameDelayItem = bitmap.GetPropertyItem(PropertyTagFrameDelay);
            }
            catch (ArgumentException) { /* Property doesn't exist, handle below */ }

            for (int i = 0; i < frameCount; i++)
            {
                bitmap.SelectActiveFrame(FrameDimension.Time, i);

                // Create NexusImage for the current frame
                // Use a copy of the frame to avoid issues with bitmap disposal/selection
                using (var frameBitmap = new Bitmap(bitmap))
                {
                     images[i] = new NexusImage(frameBitmap, processor, targetSize);
                }


                // Extract delay for this frame
                TimeSpan delay = DefaultFrameDelay; // Default if extraction fails
                if (frameDelayItem?.Value != null && frameDelayItem.Len >= (i + 1) * 4) // Each delay is 4 bytes (int32)
                {
                    // Marshal the value bytes to an integer
                     // int delayCentiseconds = BitConverter.ToInt32(frameDelayItem.Value, i * 4);
                     // Safer approach: copy relevant bytes first
                     byte[] delayBytes = new byte[4];
                     Array.Copy(frameDelayItem.Value, i * 4, delayBytes, 0, 4);
                     int delayCentiseconds = BitConverter.ToInt32(delayBytes, 0);


                    // Minimum delay is often treated as 10ms (or 1/100th sec = 10ms) by browsers,
                    // handle potential zero/small values. Let's use 10ms as minimum practical delay.
                    if (delayCentiseconds < 1) delayCentiseconds = 1; // Avoid zero delay issues
                    delay = TimeSpan.FromMilliseconds(delayCentiseconds * 10);
                }
                delays[i] = delay;
                totalDuration += delay;
            }

            return (new ReadOnlyMemory<NexusImage>(images), new ReadOnlyMemory<TimeSpan>(delays), totalDuration);
        }

        /// <summary>
        /// Gets the NexusImage frame at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the frame.</param>
        /// <returns>The NexusImage at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of bounds.</exception>
        public NexusImage this[int index]
        {
            get
            {
                if (index < 0 || index >= FrameCount)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {FrameCount - 1}.");
                }
                return _frames.Span[index];
            }
        }

         /// <summary>
        /// Gets the intended display duration for a specific frame.
        /// </summary>
        /// <param name="index">The zero-based index of the frame.</param>
        /// <returns>The TimeSpan duration for the specified frame.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if index is out of bounds.</exception>
        public TimeSpan GetFrameDelay(int index)
        {
             if (index < 0 || index >= FrameCount)
             {
                 throw new ArgumentOutOfRangeException(nameof(index), $"Index must be between 0 and {FrameCount - 1}.");
             }
             return _frameDelays.Span[index];
        }


        // --- IDisposable Implementation ---

        private bool _disposed = false;

        /// <summary>
        /// Releases resources used by the NexusAnimation, including the owned HttpClient if applicable.
        /// Note: Does not dispose the individual NexusImage frames unless they implement IDisposable themselves.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects).
                    if (_isHttpClientOwned)
                    {
                        _httpClient?.Dispose();
                        _httpClient = null;
                    }
                    // Consider if NexusImage needs disposal? If it holds GDI handles, it should.
                    // Assuming NexusImage doesn't hold unmanaged resources directly or handles its own.
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // (None in this class directly, but good practice to include the pattern)

                _disposed = true;
            }
        }

        /// <summary>
        /// Finalizer (destructor) to clean up HttpClient if Dispose wasn't called.
        /// </summary>
        ~NexusAnimation()
        {
            Dispose(false);
        }
    }

    // Assuming ImageHelper and TaskHelper exist elsewhere in your project
    internal static class ImageHelper
    {
        public static NexusSize GetSize(int originalWidth, int originalHeight, float percentage)
        {
            if (percentage <= 0f || percentage > 10.0f) // Allow some upscale, but cap it reasonably
                percentage = 1.0f;

            int newWidth = (int)Math.Max(1, originalWidth * percentage);
            // Adjust height based on console character aspect ratio (roughly 2:1 height:width)
            int newHeight = (int)Math.Max(1, originalHeight * percentage * 0.5f); // Apply aspect ratio correction
            return new NexusSize(newWidth, newHeight);
        }
    }

    // Placeholder for NexusSize if not defined elsewhere
    // public record struct NexusSize(int Width, int Height);

    // Placeholder for NexusImage if not defined elsewhere
    // public class NexusImage { public NexusSize Size { get; } /* ... other properties/methods */ }

    // Placeholder for NexusColorProcessor if not defined elsewhere
    // public abstract class NexusColorProcessor { /* ... methods to process colors */ }

     // Minimal TaskHelper replacement if needed
     internal static class TaskHelper
     {
         public static T? RunSync<T>(Func<Task<T>> func)
         {
             // Basic sync wrapper - Use with caution due to potential deadlocks
             return Task.Run(func).GetAwaiter().GetResult();
         }
     }
}
