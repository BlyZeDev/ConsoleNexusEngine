namespace ConsoleNexusEngine.Internal;

internal interface ISprite
{
    internal ReadOnlyMemory2D<CHAR_INFO> Sprite { get; }

    /// <summary>
    /// The size of the
    /// </summary>
    public NexusSize Size { get; }
}