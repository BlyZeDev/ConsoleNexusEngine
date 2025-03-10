namespace ConsoleNexusEngine.IO;

using System.Collections.Immutable;
using System.Timers;

/// <summary>
/// Represents a XInput gamepad
/// </summary>
public readonly record struct NexusGamepad
{
    /// <summary>
    /// The maximum amount of gamepads
    /// </summary>
    public const int MaxGamepads = 4;

    private static readonly ImmutableArray<Timer> _vibrationTimers;

    static NexusGamepad()
    {
        _vibrationTimers =
            [
                new Timer
                {
                    AutoReset = false,
                    Interval = 1000
                },
                new Timer
                {
                    AutoReset = false,
                    Interval = 1000
                },
                new Timer
                {
                    AutoReset = false,
                    Interval = 1000
                },
                new Timer
                {
                    AutoReset = false,
                    Interval = 1000
                }
            ];

        _vibrationTimers[0].Elapsed += (sender, args) =>
        {
            var vibration = new XINPUT_VIBRATION
            {
                wLeftMotorSpeed = 0,
                wRightMotorSpeed = 0
            };

            _ = Native.XInputSetState(0, ref vibration);
        };

        _vibrationTimers[1].Elapsed += (sender, args) =>
        {
            var vibration = new XINPUT_VIBRATION
            {
                wLeftMotorSpeed = 0,
                wRightMotorSpeed = 0
            };

            _ = Native.XInputSetState(1, ref vibration);
        };

        _vibrationTimers[2].Elapsed += (sender, args) =>
        {
            var vibration = new XINPUT_VIBRATION
            {
                wLeftMotorSpeed = 0,
                wRightMotorSpeed = 0
            };

            _ = Native.XInputSetState(2, ref vibration);
        };

        _vibrationTimers[3].Elapsed += (sender, args) =>
        {
            var vibration = new XINPUT_VIBRATION
            {
                wLeftMotorSpeed = 0,
                wRightMotorSpeed = 0
            };

            _ = Native.XInputSetState(3, ref vibration);
        };
    }

    /// <summary>
    /// Represents a not connected or empty gamepad
    /// </summary>
    public static NexusGamepad Empty => new();

    private readonly uint _id;

    /// <summary>
    /// The type of the gamepad
    /// </summary>
    public readonly NexusGamepadType Type { get; }

    /// <summary>
    /// The type of battery
    /// </summary>
    public readonly NexusBatteryType BatteryType { get; }

    /// <summary>
    /// The battery level
    /// </summary>
    public readonly NexusBatteryLevel BatteryLevel { get; }

    /// <summary>
    /// The buttons that are pressed
    /// </summary>
    public readonly NexusXInput Buttons { get; }

    /// <summary>
    /// <see langword="true"/> if the left trigger button is pressed
    /// </summary>
    public readonly bool IsLeftTriggerPressed { get; }

    /// <summary>
    /// <see langword="true"/> if the right trigger button is pressed
    /// </summary>
    public readonly bool IsRightTriggerPressed { get; }

    /// <summary>
    /// The X-coordinate of the left thumb buttons
    /// </summary>
    public readonly int LeftThumbX { get; }

    /// <summary>
    /// The Y-coordinate of the left thumb buttons
    /// </summary>
    public readonly int LeftThumbY { get; }

    /// <summary>
    /// The X-coordinate of the right thumb buttons
    /// </summary>
    public readonly int RightThumbX { get; }

    /// <summary>
    /// The Y-coordinate of the right thumb buttons
    /// </summary>
    public readonly int RightThumbY { get; }

    /// <summary>
    /// Initializes an empty <see cref="NexusGamepad"/>
    /// </summary>
    public NexusGamepad() : this(-1, NexusGamepadType.Unknown, NexusBatteryType.Unknown, NexusBatteryLevel.Empty, 0, false, false, 0, 0, 0, 0) { }

    internal NexusGamepad(in int id, in NexusGamepadType type, in NexusBatteryType batteryType, in NexusBatteryLevel batteryLevel, in NexusXInput inputs, in bool isLeftTriggerPressed, in bool isRightTriggerPressed, in int leftThumbX, in int leftThumbY, in int rightThumbX, in int rightThumbY)
    {
        _id = (uint)id;
        Type = type;
        BatteryType = batteryType;
        BatteryLevel = batteryLevel;
        Buttons = inputs;
        IsLeftTriggerPressed = isLeftTriggerPressed;
        IsRightTriggerPressed = isRightTriggerPressed;
        LeftThumbX = leftThumbX;
        LeftThumbY = leftThumbY;
        RightThumbX = rightThumbX;
        RightThumbY = rightThumbY;
    }

    /// <summary>
    /// Vibrates the gamepad
    /// </summary>
    /// <param name="lowFrequency">The vibration strength of the low-frequency motor</param>
    /// <param name="highFrequency">The vibration strength of the high-frequency motor</param>
    public void Vibrate(in NexusVibrationLevel lowFrequency, in NexusVibrationLevel highFrequency)
    {
        var vibration = new XINPUT_VIBRATION
        {
            wLeftMotorSpeed = (ushort)lowFrequency,
            wRightMotorSpeed = (ushort)highFrequency
        };

        _ = Native.XInputSetState(_id, ref vibration);

        _vibrationTimers[(int)_id].Start();
    }
}