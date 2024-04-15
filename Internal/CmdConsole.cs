namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal sealed class CmdConsole
{
    private readonly nint _handle;
    private readonly nint _standardInput;
    private readonly nint _standardOutput;

    public ConsoleBuffer Buffer { get; }

    public CmdConsole(ConsoleGameSettings settings)
    {
        _handle = Native.GetConsoleWindow();
        _standardInput = Native.GetStdHandle(-10);
        _standardOutput = Native.GetStdHandle(-11);

        var dimensions = Initialize(settings);

        Buffer = new ConsoleBuffer(dimensions.X, dimensions.Y);
    }

    public ReadOnlySpan<INexusInput> ReadInput(bool isInputAllowed)
    {
        if (!isInputAllowed)
        {
            Native.FlushConsoleInputBuffer(_standardInput);
            return [];
        }

        Native.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);

        if (numEventsRead is 0) return [];

        var buffer = new INPUT_RECORD[numEventsRead];

        Native.PeekConsoleInput(_standardInput, buffer, buffer.Length, out numEventsRead);
        Native.FlushConsoleInputBuffer(_standardInput);

        if (numEventsRead is 0) return [];

        var builder = new SpanBuilder<INexusInput>();

        INPUT_RECORD input;
        for (int i = 0; i < numEventsRead; i++)
        {
            input = buffer[i];

            switch (input.EventType)
            {
                case 1:
                    if (input.KeyEvent.KeyDown)
                        builder.Append(new KeyboardInput((NexusKey)input.KeyEvent.VirtualKeyCode));
                    break;

                case 2:
                    builder.Append(
                        new MouseInput((NexusKey)buffer[i].MouseEvent.ButtonState,
                        new Coord(
                            buffer[i].MouseEvent.MousePosition.X,
                            buffer[i].MouseEvent.MousePosition.Y)));
                    break;
            }
        }

        return builder.AsReadOnlySpan();
    }

    public void WriteInput(in ReadOnlySpan<INexusInput> inputs)
    {
        var buffer = new INPUT_RECORD[inputs.Length];

        INexusInput input;
        for (int i = 0; i < buffer.Length; i++)
        {
            input = inputs[i];

            buffer[i] = input switch
            {
                var keyboardInput when keyboardInput.MousePosition is null => new INPUT_RECORD
                {
                    EventType = 1,
                    KeyEvent = new KEY_EVENT_RECORD
                    {
                        KeyDown = true,
                        VirtualKeyCode = (ushort)keyboardInput.Key
                    }
                },
                _ => new INPUT_RECORD
                {
                    EventType = 2,
                    MouseEvent = new MOUSE_EVENT_RECORD
                    {
                        ButtonState = (uint)input.Key,
                        MousePosition = input.MousePosition!.Value.ToCOORD()
                    }
                }
            };
        }

        Native.WriteConsoleInput(_standardInput, buffer, (uint)buffer.Length, out _);
    }

    public void UpdateTitle(string title) => Native.SetWindowText(_handle, title);

    public void UpdateColorPalette(ColorPalette colorPalette)
    {
        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        foreach (var color in colorPalette.Colors)
        {
            switch (color.Key)
            {
                case ConsoleColor.Black: csbe.black = new COLORREF(color.Value); break;
                case ConsoleColor.DarkBlue: csbe.darkBlue = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGreen: csbe.darkGreen = new COLORREF(color.Value); break;
                case ConsoleColor.DarkCyan: csbe.darkCyan = new COLORREF(color.Value); break;
                case ConsoleColor.DarkRed: csbe.darkRed = new COLORREF(color.Value); break;
                case ConsoleColor.DarkMagenta: csbe.darkMagenta = new COLORREF(color.Value); break;
                case ConsoleColor.DarkYellow: csbe.darkYellow = new COLORREF(color.Value); break;
                case ConsoleColor.Gray: csbe.gray = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGray: csbe.darkGray = new COLORREF(color.Value); break;
                case ConsoleColor.Blue: csbe.blue = new COLORREF(color.Value); break;
                case ConsoleColor.Green: csbe.green = new COLORREF(color.Value); break;
                case ConsoleColor.Cyan: csbe.cyan = new COLORREF(color.Value); break;
                case ConsoleColor.Red: csbe.red = new COLORREF(color.Value); break;
                case ConsoleColor.Magenta: csbe.magenta = new COLORREF(color.Value); break;
                case ConsoleColor.Yellow: csbe.yellow = new COLORREF(color.Value); break;
                case ConsoleColor.White: csbe.white = new COLORREF(color.Value); break;
            }
        }

        Native.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);
        AdjustBufferSize(ref csbe);
    }

    public void UpdateFont(NexusFont font)
    {
        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.nFont = 0;
        
        fontInfo.dwFontSize.X = (short)font.Width;
        fontInfo.dwFontSize.Y = (short)font.Height;
        fontInfo.FaceName = font.Name;

        Native.SetCurrentConsoleFontEx(_standardOutput, false, ref fontInfo);

        AdjustBufferSize();
    }

    private Coord Initialize(ConsoleGameSettings settings)
    {
        var cursorInfo = new CONSOLE_CURSOR_INFO
        {
            bVisible = false,
            dwSize = 1
        };
        Native.SetConsoleCursorInfo(_standardOutput, ref cursorInfo);

        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.nFont = 0;

        fontInfo.dwFontSize.X = (short)settings.Font.Width;
        fontInfo.dwFontSize.Y = (short)settings.Font.Height;
        fontInfo.FaceName = settings.Font.Name;

        Native.SetCurrentConsoleFontEx(_standardOutput, false, ref fontInfo);

        Native.SetConsoleMode(_standardInput, (0x0080 | 0x0010) & ~0x0004 & ~0x0040 & ~0x0008);

        _ = Native.SetWindowLong(_handle, -16, 0x00080000);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        foreach (var color in settings.ColorPalette.Colors)
        {
            switch (color.Key)
            {
                case ConsoleColor.Black: csbe.black = new COLORREF(color.Value); break;
                case ConsoleColor.DarkBlue: csbe.darkBlue = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGreen: csbe.darkGreen = new COLORREF(color.Value); break;
                case ConsoleColor.DarkCyan: csbe.darkCyan = new COLORREF(color.Value); break;
                case ConsoleColor.DarkRed: csbe.darkRed = new COLORREF(color.Value); break;
                case ConsoleColor.DarkMagenta: csbe.darkMagenta = new COLORREF(color.Value); break;
                case ConsoleColor.DarkYellow: csbe.darkYellow = new COLORREF(color.Value); break;
                case ConsoleColor.Gray: csbe.gray = new COLORREF(color.Value); break;
                case ConsoleColor.DarkGray: csbe.darkGray = new COLORREF(color.Value); break;
                case ConsoleColor.Blue: csbe.blue = new COLORREF(color.Value); break;
                case ConsoleColor.Green: csbe.green = new COLORREF(color.Value); break;
                case ConsoleColor.Cyan: csbe.cyan = new COLORREF(color.Value); break;
                case ConsoleColor.Red: csbe.red = new COLORREF(color.Value); break;
                case ConsoleColor.Magenta: csbe.magenta = new COLORREF(color.Value); break;
                case ConsoleColor.Yellow: csbe.yellow = new COLORREF(color.Value); break;
                case ConsoleColor.White: csbe.white = new COLORREF(color.Value); break;
            }
        }

        csbe.dwSize.X = 1;
        csbe.dwSize.Y = 1;
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        Native.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Native.SetWindowPos(
            _handle,
            nint.Zero,
            0, 0,
            Native.GetSystemMetrics(0),
            Native.GetSystemMetrics(1),
            0x0040);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Native.SetWindowText(_handle, settings.Title);

        Native.SetForegroundWindow(_handle);

        return new Coord(csbe.dwSize.X, csbe.dwSize.Y);
    }

    private void AdjustBufferSize()
    {
        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        AdjustBufferSize(ref csbe);
    }

    private void AdjustBufferSize(ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe)
    {
        csbe.cbSize = Marshal.SizeOf(csbe);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        csbe.dwSize.X = 1;
        csbe.dwSize.Y = 1;
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        Native.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Native.SetWindowPos(
            _handle,
            nint.Zero,
            0, 0,
            Native.GetSystemMetrics(0),
            Native.GetSystemMetrics(1),
            0x0040);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Buffer.ChangeDimensions(csbe.dwSize.X, csbe.dwSize.Y);
    }
}