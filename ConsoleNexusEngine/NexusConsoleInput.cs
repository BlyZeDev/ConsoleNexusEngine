namespace ConsoleNexusEngine;

/// <summary>
/// The input handler for <see cref="NexusConsoleGame"/>
/// </summary>
public sealed class NexusConsoleInput
{
    private readonly CmdConsole _console;
    private readonly NexusGamepad[] _gamepads;

    private NexusCoord currentMousePos;

    /// <summary>
    /// The keys that were pressed at the last time <see cref="Update"/> was called
    /// </summary>
    public NexusKeyCollection Keys { get; }

    /// <summary>
    /// The position of the mouse at the last time <see cref="Update"/> was called
    /// </summary>
    public NexusCoord MousePosition { get; private set; }

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

        Keys = [];
    }

    /// <summary>
    /// Updates <see cref="Keys"/> and <see cref="MousePosition"/>
    /// </summary>
    public void Update() => _console.ReadKeyboardMouseInput(Keys, ref currentMousePos);

    /// <summary>
    /// Updates <see cref="Gamepad1"/>, <see cref="Gamepad2"/>, <see cref="Gamepad3"/>, <see cref="Gamepad4"/>
    /// </summary>
    public void UpdateGamepads() => _console.ReadGamepads(_gamepads);
}