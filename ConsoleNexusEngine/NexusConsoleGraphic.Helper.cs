namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Internal;

public sealed partial class NexusConsoleGraphic
{
    private void ClearSprite(in NexusCoord start, in NexusCoord end)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(end.X, end.Y);

        _console.Buffer.BlockClearChar(start.X, start.Y, end.X, end.Y);
    }

    private void DrawSprite(in NexusCoord start, ISprite sprite)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(start.X + sprite.Sprite.Width - 1, start.Y + sprite.Sprite.Height - 1);

        var destWidth = _console.Buffer.Width;
        var spriteWidth = sprite.Size.Width;
        var spriteSpan = sprite.Sprite.Span;

        for (int y = 0; y < sprite.Size.Height; y++)
        {
            _console.Buffer.BlockSetChar(spriteSpan, y * spriteWidth, (start.Y + y) * destWidth + start.X, spriteWidth);
        }
    }

    private void DrawShape(in NexusCoord start, INexusShape shape, in NexusChar character)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(start.X + shape.Size.Width - 1, start.Y + shape.Size.Height - 1);

        var drawable = shape.Draw();

        for (int x = 0; x < shape.Size.Width; x++)
        {
            for (int y = 0; y < shape.Size.Height; y++)
            {
                if (drawable[x, y]) SetChar(start.X + x, start.Y + y, character);
            }
        }
    }

    private void SetChar(in NexusCoord coordinate, in NexusChar character)
    {
        ThrowIfOutOfBounds(coordinate);

        SetChar(coordinate.X, coordinate.Y, character);
    }

    private void SetChar(in int x, in int y, in NexusChar character) => _console.Buffer.SetChar(x, y, NativeConverter.ToCharInfo(character));

    private void ThrowIfOutOfBounds(in NexusCoord coord)
        => ThrowIfOutOfBounds(coord.X, coord.Y);

    private void ThrowIfOutOfBounds(in int x, in int y)
    {
        if (x < 0 || x >= _console.Buffer.Width)
            throw new ArgumentOutOfRangeException(nameof(x), "The coordinate is not in bounds of the console buffer");
        
        if (y < 0 || y >= _console.Buffer.Height)
            throw new ArgumentOutOfRangeException(nameof(y), "The coordinate is not in bounds of the console buffer");
    }
}