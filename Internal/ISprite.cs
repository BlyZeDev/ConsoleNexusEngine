namespace ConsoleNexusEngine.Internal;

internal interface ISprite
{
    internal ReadOnlyMemory2D<NexusChar> Sprite { get; }
}