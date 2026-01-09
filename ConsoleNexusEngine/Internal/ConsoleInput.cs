namespace ConsoleNexusEngine.Internal;

internal sealed class ConsoleInput
{
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

        PInvoke.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);
        if (numEventsRead == 0) return;
        
        var bufferPtr = stackalloc INPUT_RECORD[numEventsRead];
        PInvoke.ReadConsoleInputEx(_standardInput, bufferPtr, numEventsRead, out _, PInvoke.CONSOLE_READ_NOWAIT);

        for (int i = 0; i < numEventsRead; i++)
        {
            ref readonly var current = ref bufferPtr[i];
            
            if (current.EventType == PInvoke.KEY_EVENT)
            {
                var key = (NexusKey)current.KeyEvent.VirtualKeyCode;

                if (!_reservedKeys.Contains(key))
                {
                    if (current.KeyEvent.KeyDown) Keys.AddToCurrent(key);
                    else Keys.RemoveFromCurrent(key);
                }
            }
            else if (current.EventType == PInvoke.MOUSE_EVENT)
            {
                switch (current.MouseEvent.EventFlags)
                {
                    case PInvoke.MOUSE_EVENT:
                        {
                            var buttonState = current.MouseEvent.ButtonState;

                            if ((buttonState & PInvoke.FROM_LEFT_1ST_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseLeft);
                            else Keys.AddToCurrent(NexusKey.MouseLeft);

                            if ((buttonState & PInvoke.RIGHTMOST_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseRight);
                            else Keys.AddToCurrent(NexusKey.MouseRight);

                            if ((buttonState & PInvoke.FROM_LEFT_2ND_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.MouseMiddle);
                            else Keys.AddToCurrent(NexusKey.MouseMiddle);

                            if ((buttonState & PInvoke.FROM_LEFT_3RD_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.XButton1);
                            else Keys.AddToCurrent(NexusKey.XButton1);

                            if ((buttonState & PInvoke.FROM_LEFT_4TH_BUTTON_PRESSED) == 0) Keys.RemoveFromCurrent(NexusKey.XButton2);
                            else Keys.AddToCurrent(NexusKey.XButton2);
                        }
                        break;
                    case PInvoke.MOUSE_MOVED: MousePosition = current.MouseEvent.MousePosition; break;
                    case PInvoke.MOUSE_WHEELED: Keys.AddToCurrent((current.MouseEvent.ButtonState & PInvoke.MOUSE_WHEEL_DELTA) == 0 ? NexusKey.MouseWheelUp : NexusKey.MouseWheelDown); break;
                    case PInvoke.MOUSE_HWHEELED: Keys.AddToCurrent((current.MouseEvent.ButtonState & PInvoke.MOUSE_WHEEL_DELTA) == 0 ? NexusKey.MouseWheelRight : NexusKey.MouseWheelLeft); break;
                }
            }
        }
    }
}