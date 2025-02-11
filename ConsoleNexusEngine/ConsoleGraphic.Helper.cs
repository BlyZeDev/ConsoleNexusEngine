namespace ConsoleNexusEngine;

using System;

public sealed partial class ConsoleGraphic
{
    private void DrawSprite(in NexusCoord start, ISprite sprite)
    {
        ThrowIfOutOfBounds(start);
        ThrowIfOutOfBounds(start.X + sprite.Sprite.Width, start.Y + sprite.Sprite.Height);

        BlockSetChar(MathHelper.GetIndex(start.X, start.Y, sprite.Size.Width), sprite.Sprite.Span, sprite.Size.Width); //Needs fix

        /*
        for (int x = 0; x < sprite.Sprite.Width; x++)
        {
            for (int y = 0; y < sprite.Sprite.Height; y++)
            {
                SetChar(start.X + x, start.Y + y, sprite.Sprite[x, y]);
            }
        }
        */
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

    private void SetChar(in int x, in int y, in NexusChar character) => _console.Buffer.SetChar(x, y, Converter.ToCharInfo(character));

    private unsafe void BlockSetChar(in int index, in ReadOnlySpan<CHAR_INFO> characterBlock, in int length)
        => _console.Buffer.BlockSetChar(index, characterBlock, length);

    private void ThrowIfOutOfBounds(in NexusCoord coord)
        => ThrowIfOutOfBounds(coord.X, coord.Y);

    private void ThrowIfOutOfBounds(in int x, in int y)
    {
        if (_console.Buffer.Width < 0 || _console.Buffer.Width <= x)
            throw new ArgumentOutOfRangeException(nameof(x), "The coordinate is not in bounds of the console buffer");

        if (_console.Buffer.Height < 0 || _console.Buffer.Height <= y)
            throw new ArgumentOutOfRangeException(nameof(y), "The coordinate is not in bounds of the console buffer");
    }
}