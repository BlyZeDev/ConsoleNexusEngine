namespace ConsoleNexusEngine.Graphics;

/// <summary>
/// Marks a subclass of <see cref="NexusColorPalette"/> to be ignored by <see cref="NexusConsoleGameUtil.GetRandomColorPalette(in bool)"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class IgnoreColorPaletteAttribute : Attribute { }