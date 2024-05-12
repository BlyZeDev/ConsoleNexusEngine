namespace ConsoleNexusEngine.Internal;

internal readonly record struct DefaultConsole
{
    public required bool NewlyAllocated { readonly get; init; }
    public required CONSOLE_CURSOR_INFO CursorInfo { readonly get; init; }
    public required CONSOLE_FONT_INFO_EX FontInfo { readonly get; init; }
    public required uint Mode { readonly get; init; }
    public required int WindowLong { readonly get; init; }
    public required CONSOLE_SCREEN_BUFFER_INFO_EX BufferInfo { readonly get; init; }
    public required RECT WindowRect { readonly get; init; }
    public required string WindowTitle { readonly get; init; }
}