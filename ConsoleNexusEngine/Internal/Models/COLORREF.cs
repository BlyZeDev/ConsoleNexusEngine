namespace ConsoleNexusEngine.Internal.Models;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct COLORREF
{
    internal uint ColorDWORD;

    internal COLORREF(NexusColor color)
        => ColorDWORD = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);

    internal COLORREF(uint r, uint g, uint b)
        => ColorDWORD = r + (g << 8) + (b << 16);

    internal readonly NexusColor GetColor()
        => new(
            (byte)(0x000000FFU & ColorDWORD),
            (byte)((0x0000FF00U & ColorDWORD) >> 8),
            (byte)((0x00FF0000U & ColorDWORD) >> 16));

    internal void SetColor(NexusColor color)
        => ColorDWORD = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
}