namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using ConsoleNexusEngine.Internal.Models;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// The core engine of the <see cref="ConsoleGame"/>
/// </summary>
public sealed class ConsoleEngine
{
    private readonly nint _consoleStdOutput;

    internal ConsoleEngine()
    {
        _consoleStdOutput = Native.GetConsoleStdOutput();
    }

    /// <summary>
    /// Set the font of the Console
    /// </summary>
    /// <param name="font">The font to set</param>
    public unsafe void SetFont(GameFont font)
    {
        var fontInfo = new CONSOLE_FONT_INFO_EX
        {
            cbSize = Marshal.SizeOf<CONSOLE_FONT_INFO_EX>(),
            nFont = 0,
            FontWidth = 0,
            FontHeight = (short)font.Size,
            FontFamily = 4,
            FontWeight = font.Weight
        };

        var name = font.Name.AsSpan();
        for (int i = 0; i < name.Length; i++)
        {
            fontInfo.FaceName[i] = name[i];
        }

        Native.SetConsoleFont(_consoleStdOutput, ref fontInfo);
    }
}