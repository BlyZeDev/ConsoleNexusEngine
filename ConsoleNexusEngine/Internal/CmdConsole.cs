namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;
using System.Text;

internal sealed class CmdConsole
{
    private const int STD_INPUT = -10;
    private const int STD_OUTPUT = -11;

    private readonly nint _handle;
    private readonly nint _standardInput;
    private readonly nint _standardOutput;

    private readonly DefaultConsole _defaultConsole;

    public ConsoleBuffer Buffer { get; }
    public ConsoleInput Input { get; }

    public CmdConsole(NexusConsoleGameSettings settings)
    {
        _handle = Native.GetConsoleWindow();

        var needsAllocation = _handle == nint.Zero;
        if (needsAllocation)
        {
            Native.AllocConsole();

            _handle = Native.GetConsoleWindow();
        }

        _standardInput = Native.GetStdHandle(STD_INPUT);
        _standardOutput = Native.GetStdHandle(STD_OUTPUT);

        _defaultConsole = SaveDefaultConsole(needsAllocation);

        var size = Initialize(settings);

        Buffer = new ConsoleBuffer(_standardOutput, size.Width, size.Height);
        Input = new ConsoleInput(_standardInput);
    }

    public void UpdateTitle(string title) => Native.SetWindowText(_handle, title);

    public void UpdateColorPalette(NexusColorPalette colorPalette)
    {
        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

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

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        AdjustBufferSize(ref csbe);
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

    public int MessageBox(string caption, string message, uint type) => Native.MessageBox(_handle, message, caption, type | 0x00001000);

    private DefaultConsole SaveDefaultConsole(bool newlyAllocated)
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

    private NexusSize Initialize(NexusConsoleGameSettings settings)
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

        _ = Native.SetWindowLong(_handle, -16, 0x00080000 & ~0x00100000 & ~0x00200000);

        var csbe = new CONSOLE_SCREEN_BUFFER_INFO_EX();
        csbe.cbSize = Marshal.SizeOf(csbe);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

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

        Native.SetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        var monitorInfo = new MONITORINFO
        {
            cbSize = Marshal.SizeOf<MONITORINFO>()
        };
        Native.GetMonitorInfo(Native.MonitorFromWindow(_handle, 1), ref monitorInfo);

        Native.SetWindowPos(
            _handle,
            nint.Zero,
            monitorInfo.rcWork.Left, monitorInfo.rcWork.Top,
            monitorInfo.rcWork.Right - monitorInfo.rcWork.Left,
            monitorInfo.rcWork.Bottom - monitorInfo.rcWork.Top,
            0x0040 | 0x0020);

        Native.GetConsoleScreenBufferInfoEx(_standardOutput, ref csbe);

        Native.SetWindowText(_handle, settings.Title);

        Native.SetForegroundWindow(_handle);

        return new NexusSize(csbe.dwSize.X, csbe.dwSize.Y);
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