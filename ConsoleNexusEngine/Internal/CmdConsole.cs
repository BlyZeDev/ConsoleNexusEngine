namespace ConsoleNexusEngine.Internal;

using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Text;

internal sealed class CmdConsole
{
    private const int STD_INPUT = -10;
    private const int STD_OUTPUT = -11;

    private const int MOUSE_MOVED = 0x0001;
    private const int MOUSE_WHEELED = 0x0004;
    private const int MOUSE_HWHEELED = 0x0008;

    private const int BATTERY_DEVICE_GAMEPAD = 0x00;

    private readonly nint _handle;
    private readonly nint _standardInput;
    private readonly nint _standardOutput;

    private readonly ImmutableArray<NexusKey> _mouseWheels;
    private readonly ImmutableArray<NexusKey> _mouseButtons;
    private readonly DefaultConsole _defaultConsole;

    public ConsoleBuffer Buffer { get; }

    public NexusKey StopGameKey { get; set; }

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

        _mouseWheels =
        [
            NexusKey.MouseWheelLeft,
            NexusKey.MouseWheelRight,
            NexusKey.MouseWheelDown,
            NexusKey.MouseWheelUp
        ];
        _mouseButtons =
        [
            NexusKey.MouseLeft,
            NexusKey.MouseMiddle,
            NexusKey.MouseRight,
            NexusKey.XButton1,
            NexusKey.XButton2
        ];
        _defaultConsole = SaveDefaultConsole(needsAllocation);

        var size = Initialize(settings);

        Buffer = new ConsoleBuffer(_standardOutput, size.Width, size.Height);
    }

    public void ReadKeyboardMouseInput(HashSet<NexusKey> currentPressedKeys, ref NexusCoord currentMousePos)
    {
        currentPressedKeys.ExceptWith(_mouseWheels);

        Native.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);
        if (numEventsRead == 0) return;

        var buffer = new INPUT_RECORD[numEventsRead];

        Native.PeekConsoleInput(_standardInput, buffer, numEventsRead, out _);
        Native.FlushConsoleInputBuffer(_standardInput);
        
        foreach (var input in buffer.AsSpan())
        {
            switch (input.EventType)
            {
                case 1:
                    var key = (NexusKey)input.KeyEvent.VirtualKeyCode;

                    if (input.KeyEvent.KeyDown) currentPressedKeys.Add(key);
                    else currentPressedKeys.Remove(key);
                    break;

                case 2:
                    currentMousePos = NexusCoord.FromCOORD(input.MouseEvent.MousePosition);

                    var mouseKeys = input.MouseEvent.EventFlags switch
                    {
                        MOUSE_MOVED => [],
                        MOUSE_WHEELED => [input.MouseEvent.ButtonState >> 31 == 1 ? NexusKey.MouseWheelDown : NexusKey.MouseWheelUp],
                        MOUSE_HWHEELED => [input.MouseEvent.ButtonState >> 31 == 1 ? NexusKey.MouseWheelLeft : NexusKey.MouseWheelRight],
                        _ => GetMouseButtons(input.MouseEvent.ButtonState)
                    };

                    currentPressedKeys.ExceptWith(_mouseButtons);
                    foreach (var mouseKey in mouseKeys)
                    {
                        currentPressedKeys.Add(mouseKey);
                    }
                    break;
            }
        }
    }

    public void ReadGamepads(NexusGamepad[] gamepads)
    {
        for (uint i = 0; i < NexusGamepad.MaxGamepads; i++)
        {
            gamepads[i] = GetGamepadState(i);
        }
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

    public int MessageBox(string caption, string message, in uint type) => Native.MessageBox(_handle, message, caption, type | 0x00001000 | 0x00040000);

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

        Native.SetWindowPos(
            _handle,
            nint.Zero,
            0, 0,
            Native.GetSystemMetrics(0),
            Native.GetSystemMetrics(1),
            0x0040 | 0x0020);

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

    private static ReadOnlySpan<NexusKey> GetMouseButtons(in uint buttonState)
    {
        if (buttonState == 0) return [];

        var builder = new SpanBuilder<NexusKey>();

        if ((buttonState & 0x01) != 0) builder.Append(NexusKey.MouseLeft);
        if ((buttonState & 0x02) != 0) builder.Append(NexusKey.MouseRight);
        if ((buttonState & 0x04) != 0) builder.Append(NexusKey.MouseMiddle);
        if ((buttonState & 0x08) != 0) builder.Append(NexusKey.XButton1);
        if ((buttonState & 0x10) != 0) builder.Append(NexusKey.XButton2);

        return builder.AsReadOnlySpan();
    }

    private static NexusGamepad GetGamepadState(in uint index)
    {
        var capabilites = new XINPUT_CAPABILITIES();
        _ = Native.XInputGetCapabilities(index, 0, ref capabilites);

        var battery = new XINPUT_BATTERY_INFORMATION();
        _ = Native.XInputGetBatteryInformation(index, BATTERY_DEVICE_GAMEPAD, ref battery);
        
        var state = new XINPUT_STATE();
        _ = Native.XInputGetState(index, ref state);

        var gamepad = state.Gamepad;

        return new NexusGamepad(
            (int)index,
            (NexusGamepadType)capabilites.SubType,
            (NexusBatteryType)battery.BatteryType,
            (NexusBatteryLevel)battery.BatteryLevel,
            (NexusXInput)gamepad.wButtons,
            gamepad.bLeftTrigger == byte.MaxValue,
            gamepad.bRightTrigger == byte.MaxValue,
            gamepad.sThumbLX, gamepad.sThumbLY, gamepad.sThumbRX, gamepad.sThumbRY);
    }
}