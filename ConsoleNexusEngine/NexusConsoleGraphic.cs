namespace ConsoleNexusEngine;

/// <summary>
/// The graphics engine for <see cref="NexusConsoleGame"/>
/// </summary>
public sealed class NexusConsoleGraphic
{
    private readonly CmdConsole _console;

    internal NexusConsoleGraphic(CmdConsole console) => _console = console;

    /// <summary>
    /// Gets a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be placed</param>
    /// <returns><see cref="NexusChar"/></returns>
    public NexusChar GetPixel(in NexusCoord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        return NativeConverter.ToNexusChar(_console.Buffer.GetChar(coordinate.X, coordinate.Y));
    }

    /// <summary>
    /// Draws a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be drawn</param>
    /// <param name="character">The character to draw</param>
    public void DrawPixel(in NexusCoord coordinate, in NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);

        SetChar(coordinate.X, coordinate.Y, character);
    }

    /// <summary>
    /// Draws a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text to draw</param>
    public void DrawText(in NexusCoord coordinate, NexusText text)
        => DrawSprite(coordinate, text);

    /// <summary>
    /// Draws a text in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the text should start</param>
    /// <param name="text">The text to draw</param>
    public void DrawText(in NexusCoord coordinate, NexusFiggleText text)
        => DrawSprite(coordinate, text);

    /// <summary>
    /// Draws an image in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the image</param>
    /// <param name="image">The image to draw</param>
    public void DrawImage(in NexusCoord coordinate, in NexusImage image)
        => DrawSprite(coordinate, image);

    /// <summary>
    /// Draws the current animation frame in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the animation</param>
    /// <param name="animation">The animation to draw</param>
    public void DrawAnimation(in NexusCoord coordinate, NexusAnimation animation)
        => DrawImage(coordinate, animation.NextFrame());

    /// <summary>
    /// Draws pixels in the console at specific positions
    /// </summary>
    /// <param name="character">The character to draw</param>
    /// <param name="coordinates">The coordinates where the character should be drawn</param>
    public void DrawPixels(in NexusChar character, in ReadOnlySpan<NexusCoord> coordinates)
    {
        foreach (var coordinate in coordinates)
        {
            DrawSprite(coordinate, character);
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
                SetChar(startX, y, character);
            }

            return;
        }

        if (startY == endY)
        {
            var startCoord = Math.Min(startX, endX);
            var endCoord = Math.Max(startX, endX);

            for (int x = startCoord; x <= endCoord; x++)
            {
                SetChar(x, startY, character);
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
            SetChar(startX, startY, character);

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
    /// Draws a sprite to the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the sprite</param>
    /// <param name="sprite">The sprite to draw</param>
    public void DrawSprite(in NexusCoord coordinate, INexusSprite sprite)
    {
        ThrowIfOutOfBounds(coordinate);
        ThrowIfOutOfBounds(coordinate.X + sprite.Map.Size.Width - 1, coordinate.Y + sprite.Map.Size.Height - 1);

        var spriteWidth = sprite.Map.Size.Width;
        var spriteSpan = sprite.Map._spriteMap.Span;

        for (int y = 0; y < sprite.Map.Size.Height; y++)
        {
            _console.Buffer.BlockSetChar(spriteSpan, y * spriteWidth, (coordinate.Y + y) * _console.Buffer.Width + coordinate.X, spriteWidth);
        }
    }

    /// <summary>
    /// Clears a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be cleared</param>
    public void ClearPixel(in NexusCoord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        _console.Buffer.ClearChar(coordinate.X, coordinate.Y);
    }

    /// <summary>
    /// Clears a whole column
    /// </summary>
    /// <param name="column">The column to clear</param>
    public void ClearColumn(int column)
        => ClearLine(new NexusCoord(column, 0), new NexusCoord(column, _console.Buffer.Height - 1));

    /// <summary>
    /// Clears a whole row
    /// </summary>
    /// <param name="row">The row to clear</param>
    public void ClearRow(int row)
        => ClearLine(new NexusCoord(0, row), new NexusCoord(_console.Buffer.Width - 1, row));

    /// <summary>
    /// Clears a line from one coordinate to the other
    /// </summary>
    /// <param name="start">The coordinate of the start point</param>
    /// <param name="end">The coordinate of the end point</param>
    public void ClearLine(in NexusCoord start, in NexusCoord end)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end);

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
                _console.Buffer.ClearChar(startX, y);
            }

            return;
        }

        if (startY == endY)
        {
            var startCoord = Math.Min(startX, endX);
            var endCoord = Math.Max(startX, endX);

            for (int x = startCoord; x <= endCoord; x++)
            {
                _console.Buffer.ClearChar(x, startY);
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
            _console.Buffer.ClearChar(startX, startY);

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
    /// Clears a sprite to the console at a specific position
    /// </summary>
    /// <param name="coordinate">The top left coordinates of the sprite</param>
    /// <param name="sprite">The sprite to clear</param>
    public void ClearSprite(in NexusCoord coordinate, INexusSprite sprite)
    {
        ThrowIfOutOfBounds(coordinate);
        ThrowIfOutOfBounds(coordinate.X + sprite.Map.Size.Width - 1, coordinate.Y + sprite.Map.Size.Height - 1);

        var spriteWidth = sprite.Map.Size.Width;
        Span<CHARINFO> spriteSpan = StackAlloc.Allow<CHARINFO>(sprite.Map._spriteMap.Length)
            ? stackalloc CHARINFO[sprite.Map._spriteMap.Length] : new CHARINFO[sprite.Map._spriteMap.Length];

        for (int y = 0; y < sprite.Map.Size.Height; y++)
        {
            _console.Buffer.BlockSetChar(spriteSpan, y * spriteWidth, (coordinate.Y + y) * _console.Buffer.Width + coordinate.X, spriteWidth);
        }
    }

    /// <summary>
    /// Clears the current buffer of the console
    /// </summary>
    public void Clear() => _console.Buffer.ClearBuffer();

    /// <summary>
    /// Renders the buffer to the console
    /// </summary>
    public void Render() => _console.Buffer.RenderBuffer();

    private void SetChar(int x, int y, NexusChar character) => _console.Buffer.SetChar(x, y, NativeConverter.ToCharInfo(character));

    private void ThrowIfOutOfBounds(in NexusCoord coord)
        => ThrowIfOutOfBounds(coord.X, coord.Y);

    private void ThrowIfOutOfBounds(int x, int y)
    {
        if ((uint)x >= (uint)_console.Buffer.Width)
            throw new ArgumentOutOfRangeException(nameof(x), "The coordinate is not in bounds of the console buffer");

        if ((uint)y >= (uint)_console.Buffer.Height)
            throw new ArgumentOutOfRangeException(nameof(y), "The coordinate is not in bounds of the console buffer");
    }
}