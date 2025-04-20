namespace ConsoleNexusEngine.Graphics;

using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http;

/// <summary>
/// Represents an animation that can be played in console
/// </summary>
public sealed class NexusAnimation
{
    private readonly ReadOnlyMemory<NexusImage> _images;

    private int currentFrameIndex;

    /// <summary>
    /// The size of the animation
    /// </summary>
    public NexusSize Size { get; }

    private NexusAnimation(Bitmap animation, NexusColorProcessor imageProcessor, in NexusSize? size)
    {
        if (animation.RawFormat.Guid != ImageFormat.Gif.Guid)
            throw new ArgumentException("The file has to be a gif file");

        _images = Initialize(animation, imageProcessor, size);

        Size = _images.Span[0].Sprite.Size;

        currentFrameIndex = -1;
    }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusAnimation(string filepath, NexusColorProcessor imageProcessor)
        : this(new Bitmap(filepath), imageProcessor, null) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    public NexusAnimation(Bitmap animation, NexusColorProcessor imageProcessor)
        : this(animation, imageProcessor, null) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusAnimation(string filepath, NexusColorProcessor imageProcessor, in float percentage)
        : this(new Bitmap(filepath), imageProcessor, percentage) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="percentage">The desired percentage size of the bitmap</param>
    public NexusAnimation(Bitmap animation, NexusColorProcessor imageProcessor, in float percentage)
        : this(animation, imageProcessor, ImageHelper.GetSize(animation.Width, animation.Height, percentage)) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="filepath">The path to the gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the animation</param>
    public NexusAnimation(string filepath, NexusColorProcessor imageProcessor, in NexusSize size)
        : this(new Bitmap(filepath), imageProcessor, size) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="animation">The gif file</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <param name="size">The desired size of the animation</param>
    public NexusAnimation(Bitmap animation, NexusColorProcessor imageProcessor, in NexusSize size)
        : this(animation, imageProcessor, new NexusSize?(size)) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="images">The images the animation should have</param>
    public NexusAnimation(params NexusImage[] images)
    {
        if (images.Length is 0) throw new ArgumentException("The images should be at least 1");

        _images = new ReadOnlyMemory<NexusImage>(images);
        
        currentFrameIndex = -1;
    }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="images">The images the animation should have</param>
    public NexusAnimation(in ReadOnlySpan<NexusImage> images) : this(images.ToArray()) { }

    /// <summary>
    /// Initializes a new NexusAnimation
    /// </summary>
    /// <param name="url">The url of the gif</param>
    /// <param name="imageProcessor">The image processor that should be used</param>
    /// <returns><see cref="NexusAnimation"/></returns>
    /// <exception cref="HttpRequestException"></exception>
    public static NexusAnimation FromUrl(Uri url, NexusColorProcessor imageProcessor)
    {
        using (var client = new HttpClient())
        {
            using (var stream = TaskHelper.RunSync(() => client.GetStreamAsync(url)) ?? throw new HttpRequestException("Couldn't read the image"))
            {
                return new NexusAnimation(new Bitmap(stream), imageProcessor);
            }
        }
    }

    internal NexusImage NextFrame()
    {
        currentFrameIndex++;

        if (currentFrameIndex == _images.Length) currentFrameIndex = 0;

        return _images.Span[currentFrameIndex];
    }

    private static ReadOnlyMemory<NexusImage> Initialize(Bitmap bitmap, NexusColorProcessor processor, in NexusSize? size)
    {
        var dimension = new FrameDimension(bitmap.FrameDimensionsList[0]);
        var frames = bitmap.GetFrameCount(dimension);

        var images = new NexusImage[frames];
        for (int i = 0; i < frames; i++)
        {
            bitmap.SelectActiveFrame(dimension, i);

            images[i] = new NexusImage(bitmap, processor, size);
        }
        
        return new ReadOnlyMemory<NexusImage>(images);
    }
}