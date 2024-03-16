namespace ConsoleNexusEngine;

/// <summary>
/// The graphics engine for <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleGraphic
{
    private readonly CmdConsole _console;

    private Glyph[,] glyphBuffer;

    internal int BackgroundIndex { get; private set; }

    internal ConsoleGraphic(CmdConsole console)
    {
        _console = console;

        glyphBuffer = new Glyph[_console.Width, _console.Height];

        BackgroundIndex = 0;
    }

    /// <summary>
    /// Gets a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><see cref="NexusChar"/></returns>
    public NexusChar GetPixel(Coord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        var glyph = glyphBuffer[coordinate.X, coordinate.Y];

        return new(glyph.Value, _console.ColorPalette[glyph.ForegroundIndex], _console.ColorPalette[glyph.BackgroundIndex]);
    }

    /// <summary>
    /// Gets the whole buffer of the console as 2d array
    /// </summary>
    /// <remarks>
    /// This includes characters that are not rendered yet
    /// </remarks>
    /// <returns><see cref="NexusChar"/>[,]</returns>
    public NexusChar[,] GetBuffer()
    {
        var buffer = new NexusChar[_console.Width, _console.Height];

        for (int x = 0; x < _console.Width; x++)
        {
            for (int y = 0; y < _console.Height; y++)
            {
                buffer[x, y] = NexusChar.FromGlyph(glyphBuffer[x, y], _console.ColorPalette);
            }
        }

        return buffer;
    }

    /// <summary>
    /// Sets a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <param name="character">The character itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetPixel(Coord coordinate, NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);

        var foregroundColorIndex = GetColorIndex(character.Foreground);
        var backgroundColorIndex = character.Background is null
            ? BackgroundIndex : GetColorIndex(character.Background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(character));

        SetGlyph(coordinate, new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Sets a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetText(Coord coordinate, NexusText text)
    {
        var isHorizontal = text.TextDirection is TextDirection.Horizontal;
        ThrowIfOutOfBounds(coordinate + (isHorizontal ? new Coord(text.Value.Length - 1, 0) : new Coord(0, text.Value.Length - 1)));

        var foregroundColorIndex = GetColorIndex(text.Foreground);
        var backgroundColorIndex = text.Background is null
            ? BackgroundIndex : GetColorIndex(text.Background.Value);
        
        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(text));

        var posX = -1;
        var posY = -1;
        foreach (var letter in text.Value)
        {
            if (isHorizontal) posX++;
            else posY++;

            SetGlyph(coordinate + new Coord(posX, posY), new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
        }
    }

    /// <summary>
    /// Sets a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text itself</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetText(Coord coordinate, NexusFiggleText text)
    {
        ThrowIfOutOfBounds(coordinate + new Coord(text._longestStringLength, text.Value.Length - 1));

        var foregroundColorIndex = GetColorIndex(text.Foreground);
        var backgroundColorIndex = text.Background is null
            ? BackgroundIndex : GetColorIndex(text.Background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(text));

        var posX = -1;
        var posY = -1;
        foreach (var letters in text.Value)
        {
            posY++;
            foreach (var letter in letters)
            {
                posX++;
                SetGlyph(coordinate + new Coord(posX, posY), new Glyph(letter, foregroundColorIndex, backgroundColorIndex));
            }

            posX = -1;
        }
    }

    /// <summary>
    /// Sets an image in the console at a specific position
    /// </summary>
    /// <remarks>The coordinate is the top left of the image</remarks>
    /// <param name="coordinate">The top left coordinates of the image</param>
    /// <param name="image">The image itself</param>
    public void SetImage(Coord coordinate, NexusImage image)
    {
        for (int x = 0, xCord = coordinate.X; x < image.Width; x++, xCord++)
        {
            for (int y = 0, yCord = coordinate.Y; y < image.Height; y++, yCord++)
            {
                SetPixel(new Coord(xCord, yCord), image._pixels[x, y]);
            }
        }
    }

    /// <summary>
    /// Sets a pixel in the console at specific positions
    /// </summary>
    /// <param name="character">The character itself</param>
    /// <param name="coordinates">The coordinates where the character should be placed</param>
    public void SetPixels(NexusChar character, params Coord[] coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            SetPixel(coordinate, character);
        }
    }

    /// <summary>
    /// Draws a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character itself</param>
    public void DrawLine(Coord start, Coord end, NexusChar character)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end);

        var startX = start.X;
        var startY = start.Y;
        var endX = end.X;
        var endY = end.Y;

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
            SetPixel(new Coord(startX, startY), character);

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
    /// Draws a rectangle from a start to an end point
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character itself</param>
    public void DrawRectangle(Coord start, Coord end, NexusChar character)
    {
        var bottomLeft = new Coord(start.X, end.Y);
        var topRight = new Coord(end.X, start.Y);

        DrawLine(start, bottomLeft, character);
        DrawLine(start, topRight, character);
        DrawLine(bottomLeft, end, character);
        DrawLine(topRight, end, character);
    }

    /// <summary>
    /// Fills a rectangle from a start to an end point
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character itself</param>
    public void FillRectangle(Coord start, Coord end, NexusChar character)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end);

        for (int sx = start.X; sx <= end.X; sx++)
        {
            for (int sy = start.Y; sy <= end.Y; sy++)
            {
                SetPixel(new Coord(sx, sy), character);
            }
        }
    }

    /// <summary>
    /// Set the background of the whole console to a specific color
    /// </summary>
    /// <param name="color">The color to set as background</param>
    /// <exception cref="ArgumentException"></exception>
    public void SetBackground(NexusColor color)
    {
        var index = _console.ColorPalette.GetIndex(color);

        if (index is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(color));
        
        BackgroundIndex = index;

        _console.Buffer.SetBackgroundBuffer(ref glyphBuffer, BackgroundIndex);
    }

    /// <summary>
    /// Clears a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be cleared</param>
    public void ClearAt(Coord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        SetGlyph(coordinate, GetClearGlyph());
    }

    /// <summary>
    /// Clears a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    public void ClearLine(Coord start, Coord end)
        => DrawLine(start, end, NexusChar.FromGlyph(GetClearGlyph(), _console.ColorPalette));

    /// <summary>
    /// Clears the current buffer of the console
    /// </summary>
    public void Clear()
    {
        Array.Clear(glyphBuffer);
        _console.Buffer.ClearBuffer(BackgroundIndex);
    }

    /// <summary>
    /// Renders the buffer to the console
    /// </summary>
    public void Render()
    {
        _console.Buffer.RenderBuffer();
    }

    private void SetGlyph(in Coord coord, in Glyph glyph)
    {
        glyphBuffer[coord.X, coord.Y] = glyph;
        _console.Buffer.SetBuffer(coord, glyph);
    }

    private void ThrowIfOutOfBounds(in Coord coord)
    {
        if (!glyphBuffer.IsInRange(coord))
            throw new ArgumentOutOfRangeException(nameof(coord), "The coordinate is not in bounds of the console buffer");
    }

    private Glyph GetClearGlyph() => new(char.MinValue, BackgroundIndex, BackgroundIndex);

    private int GetColorIndex(in NexusColor color)
        => _console.ColorPalette.Colors.GetKey(color);
}