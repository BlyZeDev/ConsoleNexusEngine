namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal sealed class CmdConsole
{
    private readonly nint _handle;
    private readonly nint _standardInput;
    private readonly nint _standardOutput;

    private readonly DefaultConsole _defaultConsole;

    public ConsoleBuffer Buffer { get; }
    public ConsoleInput Input { get; }

    public CmdConsole(NexusConsoleGameSettings settings)
    {
        _handle = PInvoke.GetConsoleWindow();

        var needsAllocation = _handle == nint.Zero;
        if (needsAllocation)
        {
            PInvoke.AllocConsole();

            _handle = PInvoke.GetConsoleWindow();
        }

        _standardInput = PInvoke.GetStdHandle(PInvoke.STD_INPUT_HANDLE);
        _standardOutput = PInvoke.GetStdHandle(PInvoke.STD_OUTPUT_HANDLE);

        _defaultConsole = SaveDefaultConsole(needsAllocation);

        var size = Initialize(settings);

        Buffer = new ConsoleBuffer(_standardOutput, size.Width, size.Height);
        Input = new ConsoleInput(_standardInput);
    }

    public void UpdateTitle(string title) => PInvoke.SetWindowText(_handle, title);

    public void UpdateColorPalette(NexusColorPalette colorPalette)
    {
        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        for (int i = 0; i < NexusColorPalette.MaxColorCount; i++)
        {
            switch (i)
            {
                case 0: csbe.black = new COLORREF(colorPalette[i]); break;
                case 1: csbe.darkBlue = new COLORREF(colorPalette[i]); break;
                case 2: csbe.darkGreen = new COLORREF(colorPalette[i]); break;
                case 3: csbe.darkCyan = new COLORREF(colorPalette[i]); break;
                case 4: csbe.darkRed = new COLORREF(colorPalette[i]); break;
                case 5: csbe.darkMagenta = new COLORREF(colorPalette[i]); break;
                case 6: csbe.darkYellow = new COLORREF(colorPalette[i]); break;
                case 7: csbe.gray = new COLORREF(colorPalette[i]); break;
                case 8: csbe.darkGray = new COLORREF(colorPalette[i]); break;
                case 9: csbe.blue = new COLORREF(colorPalette[i]); break;
                case 10: csbe.green = new COLORREF(colorPalette[i]); break;
                case 11: csbe.cyan = new COLORREF(colorPalette[i]); break;
                case 12: csbe.red = new COLORREF(colorPalette[i]); break;
                case 13: csbe.magenta = new COLORREF(colorPalette[i]); break;
                case 14: csbe.yellow = new COLORREF(colorPalette[i]); break;
                case 15: csbe.white = new COLORREF(colorPalette[i]); break;
            }
        }

        PInvoke.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);
        AdjustBufferSize(ref csbe);
    }

    public unsafe void UpdateFont(NexusFont font)
    {
        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.nFont = 0;
        
        fontInfo.dwFontSize.X = (short)font.Size.Width;
        fontInfo.dwFontSize.Y = (short)font.Size.Height;

        fixed (char* namePtr = font.Name)
        {
            var length = Math.Min(font.Name.Length, CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE - 1);
            System.Buffer.MemoryCopy(namePtr, fontInfo.FaceName, CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE * sizeof(char), length * sizeof(char));
            fontInfo.FaceName[CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE] = char.MinValue;
        }

        PInvoke.SetCurrentConsoleFontEx(_standardOutput, 0, ref fontInfo);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        AdjustBufferSize(ref csbe);
    }

    public void ResetToDefault()
    {
        if (_defaultConsole.NewlyAllocated)
        {
            PInvoke.FreeConsole();
            PInvoke.ShowWindow(_handle, PInvoke.SW_HIDE);
            return;
        }
        
        var cursorInfo = _defaultConsole.CursorInfo;
        PInvoke.SetConsoleCursorInfo(_standardOutput, ref cursorInfo);

        var fontInfo = _defaultConsole.FontInfo;
        PInvoke.SetCurrentConsoleFontEx(_standardOutput, 0, ref fontInfo);

        PInvoke.SetConsoleMode(_standardInput, _defaultConsole.Mode);

        PInvoke.SetWindowLong(_handle, PInvoke.GWL_STYLE, _defaultConsole.WindowLong);

        var csbe = _defaultConsole.BufferInfo;
        PInvoke.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var rect = _defaultConsole.WindowRect;
        PInvoke.SetWindowPos(
            _handle,
            PInvoke.HWND_NOTOPMOST,
            rect.Left, rect.Top,
            rect.Right - rect.Left, rect.Bottom - rect.Top,
            PInvoke.SWP_SHOWWINDOW);

        PInvoke.SetWindowText(_handle, _defaultConsole.WindowTitle);

        Console.Clear();
    }

    public int MessageBox(string caption, string message, uint type) => PInvoke.MessageBox(_handle, message, caption, type | PInvoke.MB_TOPMOST);

    private unsafe DefaultConsole SaveDefaultConsole(bool newlyAllocated)
    {
        PInvoke.GetConsoleCursorInfo(_standardOutput, out var cursorInfo);
        PInvoke.GetCurrentConsoleFontEx(_standardOutput, 0, out var fontInfo);
        PInvoke.GetConsoleMode(_standardInput, out var mode);
        var windowLong = PInvoke.GetWindowLong(_handle, PInvoke.GWL_STYLE);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);
        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var rect = new RECT();
        PInvoke.GetWindowRect(_handle, ref rect);

        var titleLength = PInvoke.GetWindowTextLength(_handle) + 1;
        var titleBuffer = stackalloc char[titleLength];
        titleLength = PInvoke.GetWindowText(_handle, titleBuffer, titleLength);

        return new DefaultConsole
        {
            NewlyAllocated = newlyAllocated,
            BufferInfo = csbe,
            CursorInfo = cursorInfo,
            FontInfo = fontInfo,
            Mode = mode,
            WindowLong = windowLong,
            WindowRect = rect,
            WindowTitle = new string(titleBuffer, 0, titleLength)
        };
    }

    private unsafe NexusSize Initialize(NexusConsoleGameSettings settings)
    {
        var cursorInfo = new CONSOLE_CURSOR_INFO
        {
            bVisible = 0,
            dwSize = 1
        };
        PInvoke.SetConsoleCursorInfo(_standardOutput, ref cursorInfo);

        var fontInfo = new CONSOLE_FONT_INFO_EX();
        fontInfo.cbSize = (uint)Marshal.SizeOf(fontInfo);
        fontInfo.nFont = 0;

        fontInfo.dwFontSize.X = (short)settings.Font.Size.Width;
        fontInfo.dwFontSize.Y = (short)settings.Font.Size.Height;

        fixed (char* namePtr = settings.Font.Name)
        {
            var length = Math.Min(settings.Font.Name.Length, CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE - 1);
            System.Buffer.MemoryCopy(namePtr, fontInfo.FaceName, CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE * sizeof(char), length * sizeof(char));
            fontInfo.FaceName[CONSOLE_FONT_INFO_EX.FACE_NAME_SIZE] = char.MinValue;
        }

        PInvoke.SetCurrentConsoleFontEx(_standardOutput, 0, ref fontInfo);

        PInvoke.SetConsoleMode(_standardInput, (PInvoke.ENABLE_EXTENDED_FLAGS | PInvoke.ENABLE_MOUSE_INPUT) & ~PInvoke.ENABLE_ECHO_INPUT & ~PInvoke.ENABLE_QUICK_EDIT_MODE & ~PInvoke.ENABLE_WINDOW_INPUT);

        _ = PInvoke.SetWindowLong(_handle, PInvoke.GWL_STYLE, PInvoke.WS_BORDER & ~PInvoke.WS_MAXIMIZE & ~PInvoke.WS_MINIMIZE);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        for (int i = 0; i < NexusColorPalette.MaxColorCount; i++)
        {
            switch (i)
            {
                case 0: csbe.black = new COLORREF(settings.ColorPalette[i]); break;
                case 1: csbe.darkBlue = new COLORREF(settings.ColorPalette[i]); break;
                case 2: csbe.darkGreen = new COLORREF(settings.ColorPalette[i]); break;
                case 3: csbe.darkCyan = new COLORREF(settings.ColorPalette[i]); break;
                case 4: csbe.darkRed = new COLORREF(settings.ColorPalette[i]); break;
                case 5: csbe.darkMagenta = new COLORREF(settings.ColorPalette[i]); break;
                case 6: csbe.darkYellow = new COLORREF(settings.ColorPalette[i]); break;
                case 7: csbe.gray = new COLORREF(settings.ColorPalette[i]); break;
                case 8: csbe.darkGray = new COLORREF(settings.ColorPalette[i]); break;
                case 9: csbe.blue = new COLORREF(settings.ColorPalette[i]); break;
                case 10: csbe.green = new COLORREF(settings.ColorPalette[i]); break;
                case 11: csbe.cyan = new COLORREF(settings.ColorPalette[i]); break;
                case 12: csbe.red = new COLORREF(settings.ColorPalette[i]); break;
                case 13: csbe.magenta = new COLORREF(settings.ColorPalette[i]); break;
                case 14: csbe.yellow = new COLORREF(settings.ColorPalette[i]); break;
                case 15: csbe.white = new COLORREF(settings.ColorPalette[i]); break;
            }
        }

        csbe.dwSize.X = 1;
        csbe.dwSize.Y = 1;
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        PInvoke.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var monitorInfo = new MONITORINFO
        {
            cbSize = Marshal.SizeOf<MONITORINFO>()
        };
        PInvoke.GetMonitorInfo(PInvoke.MonitorFromWindow(_handle, PInvoke.MONITOR_DEFAULTTONEAREST), ref monitorInfo);

        PInvoke.SetWindowPos(
            _handle,
            nint.Zero,
            monitorInfo.rcWork.Left, monitorInfo.rcWork.Top,
            monitorInfo.rcWork.Right - monitorInfo.rcWork.Left,
            monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top,
            PInvoke.SWP_SHOWWINDOW | PInvoke.SWP_FRAMECHANGED);

        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        PInvoke.SetWindowText(_handle, settings.Title);

        PInvoke.SetForegroundWindow(_handle);

        return new NexusSize(csbe.dwSize.X, csbe.dwSize.Y);
    }

    private void AdjustBufferSize(ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe)
    {
        csbe.cbSize = Marshal.SizeOf(csbe);

        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        csbe.dwSize.X = 1;
        csbe.dwSize.Y = 1;
        ++csbe.srWindow.Bottom;
        ++csbe.srWindow.Right;

        PInvoke.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var monitorInfo = new MONITORINFO
        {
            cbSize = Marshal.SizeOf<MONITORINFO>()
        };
        PInvoke.GetMonitorInfo(PInvoke.MonitorFromWindow(_handle, PInvoke.MONITOR_DEFAULTTONEAREST), ref monitorInfo);

        PInvoke.SetWindowPos(
            _handle,
            nint.Zero,
            monitorInfo.rcWork.Left, monitorInfo.rcWork.Top,
            monitorInfo.rcWork.Right - monitorInfo.rcWork.Left,
            monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top,
            PInvoke.SWP_SHOWWINDOW);

        PInvoke.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Buffer.ChangeDimensions(csbe.dwSize.X, csbe.dwSize.Y);
    }
}