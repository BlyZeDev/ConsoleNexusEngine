namespace ConsoleNexusEngine;

/// <summary>
/// The graphics engine for <see cref="ConsoleGame"/>
/// </summary>
public sealed partial class ConsoleGraphic
{
    private readonly CmdConsole _console;

    private Memory2D<NexusChar> glyphBuffer;

    internal ConsoleGraphic(CmdConsole console)
    {
        _console = console;

        glyphBuffer = new Memory2D<NexusChar>(_console.Buffer.Width, _console.Buffer.Height);

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

        return glyphBuffer[coordinate.X, coordinate.Y];
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
                buffer[x, y] = glyphBuffer[x, y];
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
        => DrawSprite(coordinate, character);

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
    /// Draws a shape from one coordinate to another
    /// </summary>
    /// <param name="start">The top left coordinate of the start point</param>
    /// <param name="shape">The shape to draw</param>
    public void DrawShape(in NexusCoord start, INexusShape shape)
    {
        if (shape is ISprite sprite)
        {
            DrawSprite(start, sprite);
            return;
        }
        
        DrawShape(start, shape, shape.Character);
    }

    /// <summary>
    /// Clears a pixel in the console at a specific position
    /// </summary>
    /// <param name="coordinate">The coordinates where the character should be cleared</param>
    public void ClearPixel(in NexusCoord coordinate)
    {
        ThrowIfOutOfBounds(coordinate);

        SetChar(coordinate, NexusChar.Empty);
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
        => DrawLine(start, end, NexusChar.Empty);

    /// <summary>
    /// Clears a shape from one coordinate to another
    /// </summary>
    /// <param name="start">The top left coordinate of the start point</param>
    /// <param name="shape">The shape to clear</param>
    public void ClearShape(in NexusCoord start, INexusShape shape)
        => DrawShape(start, shape, NexusChar.Empty);

    /// <summary>
    /// Clears the current buffer of the console
    /// </summary>
    public void Clear()
    {
        glyphBuffer.Span.Clear();
        _console.Buffer.ClearBuffer(NexusColorIndex.Background);
    }

    /// <summary>
    /// Renders the buffer to the console
    /// </summary>
    public void Render() => _console.Buffer.RenderBuffer();

    private void OnBufferUpdated(object? sender, EventArgs e)
    {
        glyphBuffer = glyphBuffer.Resize(_console.Buffer.Width, _console.Buffer.Height);

        _console.Buffer.SetBackgroundBuffer(glyphBuffer, NexusColorIndex.Background);
    }
}