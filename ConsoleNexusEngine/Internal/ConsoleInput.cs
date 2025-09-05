namespace ConsoleNexusEngine.Internal;

internal sealed class ConsoleInput
{
    private const ushort KEY_EVENT = 0x0001;
    private const ushort MOUSE_EVENT = 0x0002;

    private const uint MOUSE_CLICKED = 0x0000;
    private const uint MOUSE_MOVED = 0x0001;
    private const uint MOUSE_WHEELED = 0x0004;
    private const uint MOUSE_HWHEELED = 0x0008;

    private const uint FROM_LEFT_1ST_BUTTON_PRESSED = 0x0001;
    private const uint RIGHTMOST_BUTTON_PRESSED = 0x0002;
    private const uint FROM_LEFT_2ND_BUTTON_PRESSED = 0x0004;
    private const uint FROM_LEFT_3RD_BUTTON_PRESSED = 0x0008;
    private const uint FROM_LEFT_4TH_BUTTON_PRESSED = 0x00010;

    private readonly nint _standardInput;
    private readonly HashSet<NexusKey> _reservedKeys;

    public NexusKeyCollection Keys { get; }

    public COORD MousePosition { get; private set; }

    public ConsoleInput(nint standardInput)
    {
        _standardInput = standardInput;
        _reservedKeys =
        [
            NexusKey.LeftWindows,
            NexusKey.RightWindows,
            NexusKey.Applications
        ];

        Keys = new NexusKeyCollection();
        MousePosition = new COORD
        {
            X = 0,
            Y = 0,
        };
    }

    public unsafe void ReadInput()
    {
        Keys.InvalidateCurrent();

        Native.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);
        if (numEventsRead == 0) return;
        
        var bufferPtr = stackalloc INPUT_RECORD[numEventsRead];
        Native.ReadConsoleInputEx(_standardInput, bufferPtr, numEventsRead, out _, 0x02);

        for (int i = 0; i < numEventsRead; i++)
        {
            ref readonly var current = ref bufferPtr[i];
            
            if (current.EventType == KEY_EVENT)
            {
                var key = (NexusKey)current.KeyEvent.VirtualKeyCode;

                if (!_reservedKeys.Contains(key))
                {
                    if (current.KeyEvent.KeyDown) Keys.AddToCurrent(key);
                    else Keys.RemoveFromCurrent(key);
                }
            }
            else if (current.EventType == MOUSE_EVENT)
            {
                switch (current.MouseEvent.EventFlags)
                {
                    case MOUSE_CLICKED:
                        {
                            var buttonState = current.MouseEvent.ButtonState;

                            if ((buttonState & FROM_LEFT_1ST_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseLeft);
                            else Keys.AddToCurrent(NexusKey.MouseLeft);

                            if ((buttonState & RIGHTMOST_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseRight);
                            else Keys.AddToCurrent(NexusKey.MouseRight);

                            if ((buttonState & FROM_LEFT_2ND_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseMiddle);
                            else Keys.AddToCurrent(NexusKey.MouseMiddle);

                            if ((buttonState & FROM_LEFT_3RD_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.XButton1);
                            else Keys.AddToCurrent(NexusKey.XButton1);

                            if ((buttonState & FROM_LEFT_4TH_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.XButton2);
                            else Keys.AddToCurrent(NexusKey.XButton2);
                        }
                        break;
                    case MOUSE_MOVED: MousePosition = current.MouseEvent.MousePosition; break;
                    case MOUSE_WHEELED: Keys.AddToCurrent((current.MouseEvent.ButtonState & 0x80000000) == 0 ? NexusKey.MouseWheelUp : NexusKey.MouseWheelDown); break;
                    case MOUSE_HWHEELED: Keys.AddToCurrent((current.MouseEvent.ButtonState & 0x80000000) == 0 ? NexusKey.MouseWheelRight : NexusKey.MouseWheelLeft); break;
                }
            }
        }
    }
}