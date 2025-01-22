namespace ConsoleNexusEngine.Graphics;

using System.Drawing;

/// <summary>
/// Represents an interface for a shape
/// </summary>
public interface INexusShape
{
    internal static Pen Red => Pens.Red;

    /// <summary>
    /// The size of the shape
    /// </summary>
    public NexusSize Size { get; }

    /// <summary>
    /// The character to draw
    /// </summary>
    public NexusChar Character { get; }

    /// <summary>
    /// <see langword="true"/> if the shape is filled, otherwise <see langword="false"/>
    /// </summary>
    public bool Fill { get; }

    /// <summary>
    /// Draws on the 2D array and returns it
    /// </summary>
    /// <returns><see cref="bool"/>[,]</returns>
    public bool[,] Draw();
}