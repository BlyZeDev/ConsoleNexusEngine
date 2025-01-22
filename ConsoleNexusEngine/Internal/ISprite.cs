namespace ConsoleNexusEngine.Internal;

internal interface ISprite
{
    internal ReadOnlyMemory2D<NexusChar> Sprite { get; }

    /// <summary>
    /// The size of the
    /// </summary>
    public NexusSize Size { get; }
}