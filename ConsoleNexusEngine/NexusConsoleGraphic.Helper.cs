namespace ConsoleNexusEngine;

public sealed partial class NexusConsoleGraphic
{
    private void DrawSprite(in NexusCoord start, ISprite sprite)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(start.X + sprite.Sprite.Width, start.Y + sprite.Sprite.Height);

        var destWidth = _console.Buffer.Width;
        var spriteWidth = sprite.Size.Width;
        var spriteSpan = sprite.Sprite.Span;

        for (int y = 0; y < sprite.Size.Height; y++)
        {
            _console.Buffer.BlockSetChar(spriteSpan, y * spriteWidth, (start.Y + y) * destWidth + start.X, spriteWidth);
        }
        _console.Buffer.SetRenderArea(start.X, start.Y, start.X + sprite.Sprite.Width, start.Y + sprite.Sprite.Height);
    }

    private void DrawShape(in NexusCoord start, INexusShape shape, in NexusChar character)
    {
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
        => SetChar(coordinate.X, coordinate.Y, character);

    private void SetChar(in int x, in int y, in NexusChar character) => _console.Buffer.SetChar(x, y, NativeConverter.ToCharInfo(character));

    private void ThrowIfOutOfBounds(in NexusCoord coord)
        => ThrowIfOutOfBounds(coord.X, coord.Y);

    private void ThrowIfOutOfBounds(in int x, in int y)
    {
        if (_console.Buffer.Width < 0 || _console.Buffer.Width < x)
            throw new ArgumentOutOfRangeException(nameof(x), "The coordinate is not in bounds of the console buffer");

        if (_console.Buffer.Height < 0 || _console.Buffer.Height < y)
            throw new ArgumentOutOfRangeException(nameof(y), "The coordinate is not in bounds of the console buffer");
    }
}