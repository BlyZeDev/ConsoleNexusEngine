namespace ConsoleNexusEngine.Internal;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

internal sealed class CmdConsole
{
    private const int MOUSE_MOVED = 0x0001;
    private const int MOUSE_WHEELED = 0x0004;
    private const int MOUSE_HWHEELED = 0x0008;

    private readonly nint _handle;
    private readonly nint _standardInput;
    private readonly nint _standardOutput;
    private readonly HashSet<NexusKey> _currentPressedKeys;

    private readonly DefaultConsole _defaultConsole;

    private NexusCoord currentMousePos;

    internal bool stopGameKeyPressed;

    public ConsoleBuffer Buffer { get; }

    public CmdConsole(ConsoleGameSettings settings)
    {
        _handle = Native.GetConsoleWindow();

        var needsAllocation = _handle == nint.Zero;
        if (needsAllocation)
        {
            Native.AllocConsole();

            _handle = Native.GetConsoleWindow();
        }

        _standardInput = Native.GetStdHandle(-10);
        _standardOutput = Native.GetStdHandle(-11);

        _currentPressedKeys = [];

        _defaultConsole = SaveDefaultConsole(needsAllocation);

        currentMousePos = NexusCoord.MaxValue;
        stopGameKeyPressed = false;

        var size = Initialize(settings);

        Buffer = new ConsoleBuffer(size.Width, size.Height);
    }

    public NexusInputCollection ReadInput(in NexusKey stopGameKey, in bool inputAllowed)
    {
        if (!inputAllowed)
        {
            Native.FlushConsoleInputBuffer(_standardInput);
            return NexusInputCollection.Empty;
        }

        _currentPressedKeys.RemoveWhere(key => !IsKeyPressed(key));

        Native.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);

        if (numEventsRead is 0 && _currentPressedKeys.Count is 0) return new NexusInputCollection(currentMousePos);

        var buffer = new INPUT_RECORD[numEventsRead];

        Native.PeekConsoleInput(_standardInput, buffer, numEventsRead, out _);
        Native.FlushConsoleInputBuffer(_standardInput);
        
        foreach (var input in buffer.AsSpan())
        {
            switch (input.EventType)
            {
                case 1:
                    if (input.KeyEvent.KeyDown)
                    {
                        var key = (NexusKey)input.KeyEvent.VirtualKeyCode;

                         _currentPressedKeys.Add(key);
                        
                        stopGameKeyPressed = key == stopGameKey;
                    }
                    break;

                case 2:
                    currentMousePos = NexusCoord.FromCOORD(input.MouseEvent.MousePosition);

                    var mouseKeys = input.MouseEvent.EventFlags switch
                    {
                        MOUSE_MOVED => [],
                        MOUSE_WHEELED => [input.MouseEvent.ButtonState >> 31 is 1 ? NexusKey.MouseWheelDown : NexusKey.MouseWheelUp],
                        MOUSE_HWHEELED => [input.MouseEvent.ButtonState >> 31 is 1 ? NexusKey.MouseWheelLeft : NexusKey.MouseWheelRight],
                        _ => GetMouseButtons(input.MouseEvent.ButtonState)
                    };

                    foreach (var mouseKey in mouseKeys)
                    {
                        stopGameKeyPressed = mouseKey == stopGameKey;

                        _currentPressedKeys.Add(mouseKey);
                    }
                    break;
            }
        }

        return new NexusInputCollection(currentMousePos, _currentPressedKeys.Count is 0 ? [] : _currentPressedKeys.ToImmutableArray());
    }

    public void UpdateTitle(string title) => Native.SetWindowText(_handle, title);

    public void UpdateColorPalette(NexusColorPalette colorPalette)
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
        
        fontInfo.dwFontSize.X = (short)font.Size.Width;
        fontInfo.dwFontSize.Y = (short)font.Size.Height;
        fontInfo.FaceName = font.Name;

        Native.SetCurrentConsoleFontEx(_standardOutput, false, ref fontInfo);

        AdjustBufferSize();
    }

    public void ResetToDefault()
    {
        if (_defaultConsole.NewlyAllocated)
        {
            Native.FreeConsole();
            _ = Native.ShowWindow(_handle, 0);
            return;
        }
        
        var cursorInfo = _defaultConsole.CursorInfo;
        Native.SetConsoleCursorInfo(_standardOutput, ref cursorInfo);

        var fontInfo = _defaultConsole.FontInfo;
        Native.SetCurrentConsoleFontEx(_standardOutput, false, ref fontInfo);

        Native.SetConsoleMode(_standardInput, _defaultConsole.Mode);

        _ = Native.SetWindowLong(_handle, -16, _defaultConsole.WindowLong);

        var csbe = _defaultConsole.BufferInfo;
        Native.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var rect = _defaultConsole.WindowRect;
        Native.SetWindowPos(
            _handle,
            -2,
            rect.Left, rect.Top,
            rect.Right - rect.Left, rect.Bottom - rect.Top,
            0x0040);

        Native.SetWindowText(_handle, _defaultConsole.WindowTitle);

        Console.Clear();
    }

    private DefaultConsole SaveDefaultConsole(in bool newlyAllocated)
    {
        Native.GetConsoleCursorInfo(_standardOutput, out var cursorInfo);
        Native.GetCurrentConsoleFontEx(_standardOutput, false, out var fontInfo);
        Native.GetConsoleMode(_standardInput, out var mode);
        var windowLong = Native.GetWindowLong(_handle, -16);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);
        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var rect = new RECT();
        Native.GetWindowRect(_handle, ref rect);

        var titleBuilder = new StringBuilder(Native.GetWindowTextLength(_handle) + 1);
        _ = Native.GetWindowText(_handle, titleBuilder, titleBuilder.Capacity);

        return new DefaultConsole
        {
            NewlyAllocated = newlyAllocated,
            BufferInfo = csbe,
            CursorInfo = cursorInfo,
            FontInfo = fontInfo,
            Mode = mode,
            WindowLong = windowLong,
            WindowRect = rect,
            WindowTitle = titleBuilder.ToString()
        };
    }

    private NexusSize Initialize(ConsoleGameSettings settings)
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

        fontInfo.dwFontSize.X = (short)settings.Font.Size.Width;
        fontInfo.dwFontSize.Y = (short)settings.Font.Size.Height;
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

        return new NexusSize(csbe.dwSize.X, csbe.dwSize.Y);
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

    private static bool IsKeyPressed(in NexusKey key) => (Native.GetAsyncKeyState((int)key) & 0b1000) is not 0;

    private static ReadOnlySpan<NexusKey> GetMouseButtons(in uint buttonState)
    {
        if (buttonState is 0) return [];

        var builder = new SpanBuilder<NexusKey>();

        if ((buttonState & 0b0001) is not 0) builder.Append(NexusKey.MouseLeft);
        if ((buttonState & 0b0010) is not 0) builder.Append(NexusKey.MouseRight);
        if ((buttonState & 0b0100) is not 0) builder.Append(NexusKey.MouseMiddle);

        return builder.AsReadOnlySpan();
    }
}