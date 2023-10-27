namespace ConsoleNexusEngine.Internal;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal.Models;
using System;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial short GetAsyncKeyState(int vKey);

    [DllImport("kernel32.dll")]
    private static extern bool ReadConsoleInput(nint hConsoleInput, [Out] INPUT_RECORD[] lpBuffer, uint nLength, out uint lpNumberOfEventsRead);
    
    public static bool IsKeyPressed(NexusKey key)
        => (GetAsyncKeyState((int)key) & 0x8000) != 0;

    public static ReadOnlySpan<INexusInput> ReadConsoleInput(nint stdInput)
    {
        var buffer = new INPUT_RECORD[128];
        _ = ReadConsoleInput(stdInput, buffer, (uint)buffer.Length, out var numEventsRead);

        var builder = new SpanBuilder<INexusInput>();

        if (numEventsRead > 0)
        {
            for (int i = 0; i < numEventsRead; i++)
            {
                switch (buffer[i].EventType)
                {
                    case 1:
                        if (buffer[i].KeyEvent.KeyDown)
                            builder.Append(
                                new KeyboardInput(
                                    DateTime.Now,
                                    (NexusKey)buffer[i].KeyEvent.VirtualKeyCode));
                        break;

                    case 2:
                        builder.Append(
                            new MouseInput(
                                DateTime.Now,
                                (NexusKey)buffer[i].MouseEvent.ButtonState,
                                new Coord(buffer[i].MouseEvent.MousePosition.X, buffer[i].MouseEvent.MousePosition.Y)));
                        break;
                }
            }
        }

        return builder.AsReadOnlySpan();
    }
}