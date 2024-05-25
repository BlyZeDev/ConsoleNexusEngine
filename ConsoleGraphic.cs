namespace ConsoleNexusEngine;

/// <summary>
/// The graphics engine for <see cref="ConsoleGame"/>
/// </summary>
public sealed partial class ConsoleGraphic
{
    private readonly CmdConsole _console;
    private readonly ConsoleGameSettings _settings;

    private Memory2D<Glyph> glyphBuffer;

    internal int BackgroundIndex { get; private set; }

    internal ConsoleGraphic(CmdConsole console, ConsoleGameSettings settings)
    {
        _console = console;
        _settings = settings;

        glyphBuffer = new Memory2D<Glyph>(_console.Buffer.Width, _console.Buffer.Height);

        BackgroundIndex = 0;

        _console.Buffer.Updated += OnBufferUpdated;
    }

    /// <summary>
    /// Gets a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <returns><see cref="NexusChar"/></returns>
    public NexusChar GetPixel(in NexusCoord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        var glyph = glyphBuffer[coordinate.X, coordinate.Y];

        return new(glyph.Value, _settings.ColorPalette[glyph.ForegroundIndex], _settings.ColorPalette[glyph.BackgroundIndex]);
    }

    /// <summary>
    /// Gets the whole buffer of the console as two-dimensional array
    /// </summary>
    /// <remarks>
    /// This includes characters that are not rendered yet
    /// </remarks>
    /// <returns><see cref="NexusChar"/>[,]</returns>
    public NexusChar[,] GetBuffer()
    {
        var buffer = new NexusChar[_console.Buffer.Width, _console.Buffer.Height];

        for (int x = 0; x < _console.Buffer.Width; x++)
        {
            for (int y = 0; y < _console.Buffer.Height; y++)
            {
                buffer[x, y] = NexusChar.FromGlyph(glyphBuffer[x, y], _settings.ColorPalette);
            }
        }

        return buffer;
    }

    /// <summary>
    /// Draws a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be drawn</param>
    /// <param name="character">The character to draw</param>
    public void DrawPixel(in NexusCoord coordinate, in NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);
        GetOrThrowColorIndex(character.Foreground, character.Background, nameof(character), out var foregroundColorIndex, out var backgroundColorIndex);

        SetGlyph(coordinate, new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Draws a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text to draw</param>
    public void DrawText(in NexusCoord coordinate, NexusText text)
    {
        var isHorizontal = text.TextDirection is NexusTextDirection.Horizontal;

        ThrowIfOutOfBounds(coordinate + (isHorizontal ? new NexusCoord(text.Value.Length - 1, 0) : new NexusCoord(0, text.Value.Length - 1)));
        GetOrThrowColorIndex(text.Foreground, text.Background, nameof(text), out var foregroundColorIndex, out var backgroundColorIndex);

        var posX = isHorizontal ? -1 : 0;
        var posY = isHorizontal ? 0 : -1;
        foreach (var letter in text.Value.AsSpan())
        {
            if (isHorizontal) posX++;
            else posY++;

            SetGlyph(coordinate.X + posX, coordinate.Y + posY, new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
        }
    }

    /// <summary>
    /// Draws a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text to draw</param>
    public void DrawText(in NexusCoord coordinate, NexusFiggleText text)
    {
        ThrowIfOutOfBounds(coordinate + new NexusCoord(text._longestStringLength, text.Value.Count - 1));
        GetOrThrowColorIndex(text.Foreground, text.Background, nameof(text), out var foregroundColorIndex, out var backgroundColorIndex);
        
        var posX = -1;
        var posY = -1;
        foreach (var letters in text._value.AsSpan())
        {
            posY++;
            foreach (var letter in letters.AsSpan())
            {
                posX++;
                SetGlyph(coordinate.X + posX, coordinate.Y + posY, new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
            }

            posX = -1;
        }
    }

    /// <summary>
    /// Draws an image in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the image</param>
    /// <param name="image">The image to draw</param>
    public void DrawImage(in NexusCoord coordinate, in NexusImage image)
    {
        ThrowIfOutOfBounds(coordinate);
        ThrowIfOutOfBounds(new NexusCoord(image.Width - 1, image.Height - 1));

        NexusChar currentPixel;
        for (int x = 0, xCord = coordinate.X; x < image.Width; x++, xCord++)
        {
            for (int y = 0, yCord = coordinate.Y; y < image.Height; y++, yCord++)
            {
                currentPixel = image[x, y];

                GetOrThrowColorIndex(currentPixel.Foreground, currentPixel.Background, nameof(image), out var foregroundColorIndex, out var backgroundColorIndex);

                SetGlyph(xCord, yCord, new Glyph(currentPixel.Value, foregroundColorIndex, backgroundColorIndex));
            }
        }
    }

    /// <summary>
    /// Draws the current animation frame in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the animation</param>
    /// <param name="animation">The animation to draw</param>
    public void DrawAnimation(in NexusCoord coordinate, NexusAnimation animation) => DrawImage(coordinate, animation.NextFrame());

    /// <summary>
    /// Draws pixels in the console at specific positions
    /// </summary>
    /// <param name="character">The character to draw</param>
    /// <param name="coordinates">The coordinates where the character should be drawn</param>
    public void DrawPixels(in NexusChar character, in ReadOnlySpan<NexusCoord> coordinates)
    {
        GetOrThrowColorIndex(character.Foreground, character.Background, nameof(character), out var foregroundColorIndex, out var backgroundColorIndex);

        var glyph = new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex);

        foreach (var coordinate in coordinates)
        {
            ThrowIfOutOfBounds(coordinate);
            SetGlyph(coordinate, glyph);
        }
    }

    /// <summary>
    /// Draws a pixel in the console at specific positions
    /// </summary>
    /// <param name="character">The character to draw</param>
    /// <param name="coordinates">The coordinates where the character should be drawn</param>
    public void DrawPixels(in NexusChar character, params NexusCoord[] coordinates)
        => DrawPixels(character, coordinates.AsSpan());

    /// <summary>
    /// Draws a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character to draw</param>
    public void DrawLine(in NexusCoord start, in NexusCoord end, in NexusChar character)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end);
        GetOrThrowColorIndex(character.Foreground, character.Background, nameof(character), out var foregroundColorIndex, out var backgroundColorIndex);

        var glyph = new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex);

        var startX = start.X;
        var startY = start.Y;
        var endX = end.X;
        var endY = end.Y;

        if (startX == endX)
        {
            var startCoord = Math.Min(startY, endY);
            var endCoord = Math.Max(startY, endY);

            for (int y = startCoord; y <= endCoord; y++)
            {
                SetGlyph(startX, y, glyph);
            }

            return;
        }

        if (startY == endY)
        {
            var startCoord = Math.Min(startX, endX);
            var endCoord = Math.Max(startX, endX);

            for (int x = startCoord; x <= endCoord; x++)
            {
                SetGlyph(x, startY, glyph);
            }

            return;
        }

        var width = endX - startX;
        var height = endY - startY;

        int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;

        if (width < 0) dx1 = -1;
        else if (width > 0) dx1 = 1;

        if (height < 0) dy1 = -1;
        else if (height > 0) dy1 = 1;

        if (width < 0) dx2 = -1;
        else if (width > 0) dx2 = 1;

        var longest = Math.Abs(width);
        var shortest = Math.Abs(height);

        if (!(longest > shortest))
        {
            longest = Math.Abs(height);
            shortest = Math.Abs(width);

            if (height < 0) dy2 = -1;
            else if (height > 0) dy2 = 1;

            dx2 = 0;
        }

        var numerator = longest >> 1;

        for (int i = 0; i <= longest; i++)
        {
            SetGlyph(startX, startY, glyph);

            numerator += shortest;

            if (!(numerator < longest))
            {
                numerator -= longest;
                startX += dx1;
                startY += dy1;
            }
            else
            {
                startX += dx2;
                startY += dy2;
            }
        }
    }

    /// <summary>
    /// Draws a shape from one coordinate to another
    /// </summary>
    /// <param name="start">The top left coordinate of the start point</param>
    /// <param name="shape">The shape to draw</param>
    public void DrawShape(in NexusCoord start, INexusShape shape)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(new NexusCoord(start.X + shape.Size.Width, start.Y + shape.Size.Height));
        GetOrThrowColorIndex(shape.Character.Foreground, shape.Character.Background, nameof(shape.Character), out var foregroundColorIndex, out var backgroundColorIndex);

        DrawShape(start, shape, new Glyph(shape.Character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Set the background of the console to a specific color
    /// </summary>
    /// <param name="color">The color to set as background</param>
    public void SetBackground(in NexusColor color)
    {
        var index = _settings.ColorPalette.Colors.GetKey(color);

        BackgroundIndex = index is -1 ? throw new ArgumentException("The color is not in the color palette", nameof(color)) : index;

        _console.Buffer.SetBackgroundBuffer(glyphBuffer, BackgroundIndex);
    }

    /// <summary>
    /// Clears a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be cleared</param>
    public void ClearPixel(in NexusCoord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        SetGlyph(coordinate, GetClearGlyph());
    }

    /// <summary>
    /// Clears a whole column
    /// </summary>
    /// <param name="column">The column to clear</param>
    public void ClearColumn(in int column)
        => ClearLine(new NexusCoord(column, 0), new NexusCoord(column, _console.Buffer.Height - 1));

    /// <summary>
    /// Clears a whole row
    /// </summary>
    /// <param name="row">The row to clear</param>
    public void ClearRow(in int row)
        => ClearLine(new NexusCoord(0, row), new NexusCoord(_console.Buffer.Width - 1, row));

    /// <summary>
    /// Clears a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    public void ClearLine(in NexusCoord start, in NexusCoord end)
        => DrawLine(start, end, NexusChar.FromGlyph(GetClearGlyph(), _settings.ColorPalette));

    /// <summary>
    /// Clears a shape from one coordinate to another
    /// </summary>
    /// <param name="start">The top left coordinate of the start point</param>
    /// <param name="shape">The shape to clear</param>
    public void ClearShape(in NexusCoord start, INexusShape shape)
        => DrawShape(start, shape, GetClearGlyph());

    /// <summary>
    /// Clears the current buffer of the console
    /// </summary>
    public void Clear()
    {
        glyphBuffer.Span.Clear();
        _console.Buffer.ClearBuffer(BackgroundIndex);
    }

    /// <summary>
    /// Renders the buffer to the console
    /// </summary>
    public void Render() => _console.Buffer.RenderBuffer();

    private void OnBufferUpdated(object? sender, EventArgs e)
    {
        glyphBuffer = glyphBuffer.Resize(_console.Buffer.Width, _console.Buffer.Height);

        SetBackground(_settings.ColorPalette[BackgroundIndex]);
    }
}