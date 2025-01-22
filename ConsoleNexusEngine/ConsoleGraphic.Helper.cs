namespace ConsoleNexusEngine;

public sealed partial class ConsoleGraphic
{
    private void DrawSprite(in NexusCoord start, ISprite sprite)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(start.X + sprite.Sprite.Width, start.Y + sprite.Sprite.Height);

        for (int x = 0; x < sprite.Sprite.Width; x++)
        {
            for (int y = 0; y < sprite.Sprite.Height; y++)
            {
                SetChar(start.X + x, start.Y + y, sprite.Sprite[x, y]);
            }
        }
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

    private void SetChar(in int x, in int y, in NexusChar character)
    {
        glyphBuffer[x, y] = character;
        _console.Buffer.SetChar(x, y, character);
    }

    private void SetChar(in NexusCoord coordinate, in NexusChar character)
        => SetChar(coordinate.X, coordinate.Y, character);

    private void ThrowIfOutOfBounds(in NexusCoord coord)
        => ThrowIfOutOfBounds(coord.X, coord.Y);

    private void ThrowIfOutOfBounds(in int x, in int y)
    {
        var isInRange = glyphBuffer.IsInRange(x, y);

        if (!isInRange.X)
            throw new ArgumentOutOfRangeException(nameof(x), "The coordinate is not in bounds of the console buffer");

        if (!isInRange.Y)
            throw new ArgumentOutOfRangeException(nameof(y), "The coordinate is not in bounds of the console buffer");
    }
}