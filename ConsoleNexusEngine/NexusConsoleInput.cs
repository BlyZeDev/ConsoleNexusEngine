namespace ConsoleNexusEngine;

/// <summary>
/// The input handler for <see cref="NexusConsoleGame"/>
/// </summary>
public sealed class NexusConsoleInput
{
    private readonly CmdConsole _console;
    private readonly NexusGamepad[] _gamepads;

    /// <summary>
    /// The keyboard information at the last time <see cref="Update"/> was called
    /// </summary>
    public NexusKeyCollection Keys => _console.Input.Keys;

    /// <summary>
    /// The position of the mouse at the last time <see cref="Update"/> was called
    /// </summary>
    public NexusCoord MousePosition => NativeConverter.ToNexusCoord(_console.Input.MousePosition);

    /// <summary>
    /// The input of gamepad 1 at the last time <see cref="UpdateGamepads"/> was called
    /// </summary>
    public NexusGamepad Gamepad1 => _gamepads[0];

    /// <summary>
    /// The input of gamepad 2 at the last time <see cref="UpdateGamepads"/> was called
    /// </summary>
    public NexusGamepad Gamepad2 => _gamepads[1];

    /// <summary>
    /// The input of gamepad 3 at the last time <see cref="UpdateGamepads"/> was called
    /// </summary>
    public NexusGamepad Gamepad3 => _gamepads[2];

    /// <summary>
    /// The input of gamepad 4 at the last time <see cref="UpdateGamepads"/> was called
    /// </summary>
    public NexusGamepad Gamepad4 => _gamepads[3];

    internal NexusConsoleInput(CmdConsole console)
    {
        _console = console;
        _gamepads = new NexusGamepad[4];
    }

    /// <summary>
    /// Updates <see cref="Keys"/> and <see cref="MousePosition"/>
    /// </summary>
    public void Update() => _console.Input.ReadInput();

    /// <summary>
    /// Updates <see cref="Gamepad1"/>, <see cref="Gamepad2"/>, <see cref="Gamepad3"/>, <see cref="Gamepad4"/>
    /// </summary>
    public void UpdateGamepads()
    {
        for (uint i = 0; i < NexusGamepad.MaxGamepads; i++)
        {
            _gamepads[i] = GetGamepadState(i);
        }
    }

    private static NexusGamepad GetGamepadState(uint index)
    {
        var state = new XINPUT_STATE();
        if (Native.XInputGetState(index, ref state) != 0) return NexusGamepad.Empty;

        var capabilites = new XINPUT_CAPABILITIES();
        _ = Native.XInputGetCapabilities(index, 0, ref capabilites);

        var battery = new XINPUT_BATTERY_INFORMATION();
        _ = Native.XInputGetBatteryInformation(index, 0x00, ref battery);

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