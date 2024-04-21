namespace ConsoleNexusEngine;

/// <summary>
/// The graphics engine for <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleGraphic
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
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns><see cref="NexusChar"/></returns>
    public NexusChar GetPixel(Coord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        var glyph = glyphBuffer[coordinate.X, coordinate.Y];

        return new(glyph.Value, _settings.ColorPalette[glyph.ForegroundIndex], _settings.ColorPalette[glyph.BackgroundIndex]);
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
    /// Sets a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <param name="character">The character to draw</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void DrawPixel(Coord coordinate, NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);
        GetOrThrowColorIndex(character.Foreground, character.Background, nameof(character), out var foregroundColorIndex, out var backgroundColorIndex);

        SetGlyph(coordinate, new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex));
    }

    /// <summary>
    /// Sets a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text to draw</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void DrawText(Coord coordinate, NexusText text)
    {
        var isHorizontal = text.TextDirection is TextDirection.Horizontal;
        ThrowIfOutOfBounds(coordinate + (isHorizontal ? new Coord(text.Value.Length - 1, 0) : new Coord(0, text.Value.Length - 1)));
        GetOrThrowColorIndex(text.Foreground, text.Background, nameof(text), out var foregroundColorIndex, out var backgroundColorIndex);

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
    /// <param name="text">The text to draw</param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void DrawText(Coord coordinate, NexusFiggleText text)
    {
        ThrowIfOutOfBounds(coordinate + new Coord(text._longestStringLength, text.Value.Length - 1));
        GetOrThrowColorIndex(text.Foreground, text.Background, nameof(text), out var foregroundColorIndex, out var backgroundColorIndex);

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
    /// <param name="image">The image to draw</param>
    public void DrawImage(Coord coordinate, NexusImage image)
    {
        ThrowIfOutOfBounds(coordinate);
        ThrowIfOutOfBounds(new Coord(image.Width - 1, image.Height - 1));

        NexusChar currentPixel;
        for (int x = 0, xCord = coordinate.X; x < image.Width; x++, xCord++)
        {
            for (int y = 0, yCord = coordinate.Y; y < image.Height; y++, yCord++)
            {
                currentPixel = image[x, y];

                GetOrThrowColorIndex(currentPixel.Foreground, currentPixel.Background, nameof(image), out var foregroundColorIndex, out var backgroundColorIndex);

                SetGlyph(new Coord(xCord, yCord), new Glyph(image[x, y].Value, foregroundColorIndex, backgroundColorIndex));
            }
        }
    }

    /// <summary>
    /// Sets the current animation frame in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the animation</param>
    /// <param name="animation">The animation to draw</param>
    public void DrawAnimation(Coord coordinate, NexusAnimation animation) => DrawImage(coordinate, animation.NextFrame());

    /// <summary>
    /// Sets a pixel in the console at specific positions
    /// </summary>
    /// <param name="character">The character to draw</param>
    /// <param name="coordinates">The coordinates where the character should be placed</param>
    public void DrawPixels(NexusChar character, in ReadOnlySpan<Coord> coordinates)
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
    /// Sets a pixel in the console at specific positions
    /// </summary>
    /// <param name="character">The character to draw</param>
    /// <param name="coordinates">The coordinates where the character should be placed</param>
    public void DrawPixels(NexusChar character, params Coord[] coordinates)
        => DrawPixels(character, coordinates.AsSpan());

    /// <summary>
    /// Draws a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    /// <param name="character">The character to draw</param>
    public void DrawLine(Coord start, Coord end, NexusChar character)
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
                SetGlyph(new Coord(startX, y), glyph);
            }

            return;
        }

        if (startY == endY)
        {
            var startCoord = Math.Min(startX, endX);
            var endCoord = Math.Max(startX, endX);

            for (int x = startCoord; x <= endCoord; x++)
            {
                SetGlyph(new Coord(x, startY), glyph);
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
            SetGlyph(new Coord(startX, startY), glyph);

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
    /// <param name="shape">The shape to draw</param>
    /// <param name="start">The coordinate of the start of the point</param>
    /// <param name="end">The coordinate of the end of the point</param>
    /// <param name="character">The character to draw</param>
    public void DrawShape(NexusShape shape, Coord start, Coord end, NexusChar character)
    {
        switch (shape)
        {
            case NexusShape.Rectangle: DrawRectangle(start, end, character); break;
            case NexusShape.Ellipse: DrawEllipse(start, end, character); break;
        }
    }

    /// <summary>
    /// Fills a shape from one coordinate to another
    /// </summary>
    /// <param name="shape">The shape to draw</param>
    /// <param name="start">The coordinate of the start of the point</param>
    /// <param name="end">The coordinate of the end of the point</param>
    /// <param name="character">The character to draw</param>
    public void FillShape(NexusShape shape, Coord start, Coord end, NexusChar character)
    {
        switch (shape)
        {
            case NexusShape.Rectangle: FillRectangle(start, end, character); break;
        }
    }

    /// <summary>
    /// Set the background of the whole console to a specific color
    /// </summary>
    /// <param name="color">The color to set as background</param>
    /// <exception cref="ArgumentException"></exception>
    public void SetBackground(NexusColor color)
    {
        var index = _settings.ColorPalette.Colors.GetKey(color);

        if (index is -1)
            throw new ArgumentException("The color is not in the color palette", nameof(color));

        BackgroundIndex = index;

        _console.Buffer.SetBackgroundBuffer(glyphBuffer, BackgroundIndex);
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
    /// Clears a specific whole column
    /// </summary>
    /// <param name="column">The column to clear</param>
    public void ClearColumn(int column)
        => ClearLine(new Coord(column, 0), new Coord(column, _console.Buffer.Height - 1));

    /// <summary>
    /// Clears a specific whole row
    /// </summary>
    /// <param name="row">The row to clear</param>
    public void ClearRow(int row)
        => ClearLine(new Coord(0, row), new Coord(_console.Buffer.Width - 1, row));

    /// <summary>
    /// Clears a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    public void ClearLine(Coord start, Coord end)
        => DrawLine(start, end, NexusChar.FromGlyph(GetClearGlyph(), _settings.ColorPalette));

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

    private void DrawRectangle(in Coord start, in Coord end, in NexusChar character)
    {
        var bottomLeft = new Coord(start.X, end.Y);
        var topRight = new Coord(end.X, start.Y);

        DrawLine(start, bottomLeft, character);
        DrawLine(start, topRight, character);
        DrawLine(bottomLeft, end, character);
        DrawLine(topRight, end, character);
    }

    private void DrawEllipse(in Coord start, in Coord end, in NexusChar character)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end);
        GetOrThrowColorIndex(character.Foreground, character.Background, nameof(character), out var foregroundColorIndex, out var backgroundColorIndex);

        var glyph = new Glyph(character.Value, foregroundColorIndex, backgroundColorIndex);

        var width = end.X - start.X;
        var height = end.Y - start.Y;

        var centerX = width / 2;
        var centerY = height / 2;

        var radiusX = start.X + centerX;
        var radiusY = start.Y + centerY;

        double dx, dy, d1, d2, x, y;
        x = 0;
        y = centerY;

        d1 = centerY * centerY - centerX * centerX * centerY + 0.25 * centerX * centerX;
        dx = 2 * centerY * centerY * x;
        dy = 2 * centerX * centerX * y;

        while (dx < dy)
        {
            SetGlyph(new Coord((int)(x + radiusX), (int)(y + radiusY)), glyph);
            SetGlyph(new Coord((int)(-x + radiusX), (int)(y + radiusY)), glyph);
            SetGlyph(new Coord((int)(x + radiusX), (int)(-y + radiusY)), glyph);
            SetGlyph(new Coord((int)(-x + radiusX), (int)(-y + radiusY)), glyph);

            if (d1 < 0)
            {
                x++;
                dx += 2 * centerY * centerY;
                d1 = d1 + dx + centerY * centerY;
            }
            else
            {
                x++;
                y--;
                dx += 2 * centerY * centerY;
                dy -= 2 * centerX * centerX;
                d1 = d1 + dx - dy + centerY * centerY;
            }
        }

        d2 = centerY * centerY * ((x + 0.5) * (x + 0.5)) + centerX * centerX * ((y - 1) * (y - 1)) - centerX * centerX * centerY * centerY;

        while (y >= 0)
        {
            SetGlyph(new Coord((int)(x + radiusX), (int)(y + radiusY)), glyph);
            SetGlyph(new Coord((int)(-x + radiusX), (int)(y + radiusY)), glyph);
            SetGlyph(new Coord((int)(x + radiusX), (int)(-y + radiusY)), glyph);
            SetGlyph(new Coord((int)(-x + radiusX), (int)(-y + radiusY)), glyph);

            if (d2 > 0)
            {
                y--;
                dy -= 2 * centerX * centerX;
                d2 = d2 + centerX * centerX - dy;
            }
            else
            {
                y--;
                x++;
                dx += 2 * centerY * centerY;
                dy -= 2 * centerX * centerX;
                d2 = d2 + dx - dy + centerX * centerX;
            }
        }
    }

    private void FillRectangle(in Coord start, in Coord end, in NexusChar character)
    {
        for (int y = start.Y; y <= end.Y; y++)
        {
            DrawLine(new Coord(start.X, y), new Coord(end.X, y), character);
        }
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

    private void GetOrThrowColorIndex(in NexusColor foreground, in NexusColor? background, string paramName, out int foregroundColorIndex, out int backgroundColorIndex)
    {
        foregroundColorIndex = GetColorIndex(foreground);
        backgroundColorIndex = background is null ? BackgroundIndex : GetColorIndex(background.Value);

        if (foregroundColorIndex is -1 || backgroundColorIndex is -1)
            throw new ArgumentException("The color is not in the color palette", paramName);
    }

    private Glyph GetClearGlyph() => new(char.MinValue, BackgroundIndex, BackgroundIndex);

    private int GetColorIndex(in NexusColor color)
        => _settings.ColorPalette.Colors.GetKey(color);
}