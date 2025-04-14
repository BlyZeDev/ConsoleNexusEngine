namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Marks a subclass of <see cref="NexusColorPalette"/> to be included by <see cref="NexusConsoleGameUtil.GetRandomColorPalette(in bool)"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IncludeColorPaletteAttribute : Attribute { }