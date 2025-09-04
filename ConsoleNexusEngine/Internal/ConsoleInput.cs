namespace ConsoleNexusEngine.Internal;

using System.Text;

internal sealed class ConsoleInput
{
    private const int KEY_EVENT = 0x0001;
    private const int MOUSE_EVENT = 0x0002;

    private readonly nint _standardInput;

    public NexusKeyCollection Keys { get; }

    public COORD MousePosition { get; private set; }

    public ConsoleInput(nint standardInput)
    {
        _standardInput = standardInput;

        Keys = new NexusKeyCollection();
        MousePosition = new COORD
        {
            X = 0,
            Y = 0,
        };
    }

    public unsafe void ReadInput()
    {
        Native.GetNumberOfConsoleInputEvents(_standardInput, out var numEventsRead);
        if (numEventsRead == 0) return;

        var bufferPtr = stackalloc INPUT_RECORD[numEventsRead];
        Native.ReadConsoleInputEx(_standardInput, bufferPtr, numEventsRead, out _, 0x02);

        for (int i = 0; i < numEventsRead; i++)
        {
            ref readonly var current = ref bufferPtr[i];

            if (current.EventType == KEY_EVENT)
            {
                //TODO: https://learn.microsoft.com/de-de/windows/console/key-event-record-str
            }
            else if (current.EventType == MOUSE_EVENT)
            {
                MousePosition = current.MouseEvent.MousePosition;

                //TODO: https://learn.microsoft.com/de-de/windows/console/mouse-event-record-str
            }
        }
    }

    private static void DebugKeyboard(INPUT_RECORD current)
    {
        System.IO.File.AppendAllText(@"C:\Users\leons\Downloads\testlog.txt",
            $"""
            Timestamp: {Environment.TickCount64}
            ------------------------------------------------
            ---- Keyboard ----
            KeyDown: {current.KeyEvent.KeyDown}
            VirtualKeyCode: {current.KeyEvent.VirtualKeyCode}
            VirtualScanCode: {current.KeyEvent.VirtualScanCode}
            ControlKeyState: {current.KeyEvent.ControlKeyState}
            RepeatCount: {current.KeyEvent.RepeatCount}
            UnicodeChar: {current.KeyEvent.UnicodeChar}
            ------------------------------------------------

            """, Encoding.UTF8);
    }

    private static void DebugMouse(INPUT_RECORD current)
    {
        System.IO.File.AppendAllText(@"C:\Users\leons\Downloads\testlog.txt",
            $"""
            Timestamp: {Environment.TickCount64}
            ------------------------------------------------
            ---- Mouse ----
            MousePos: {current.MouseEvent.MousePosition.X}|{current.MouseEvent.MousePosition.Y}
            ButtonState: {current.MouseEvent.ButtonState}
            EventFlags: {current.MouseEvent.EventFlags}
            ControlKeyState: {current.MouseEvent.ControlKeyState}
            ------------------------------------------------

            """, Encoding.UTF8);
    }
}