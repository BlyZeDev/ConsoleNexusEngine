namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents a XInput type
/// </summary>
public enum NexusGamepadType
{
    /// <summary>
    /// The controller type is unknown
    /// </summary>
    Unknown = 0x00,
    /// <summary>
    /// Gamepad controller.<br/>Includes Left and Right Sticks, Left and Right Triggers, Directional Pad, and all standard buttons (A, B, X, Y, START, BACK, LB, RB, LSB, RSB).
    /// </summary>
    Gamepad = 0x01,
    /// <summary>
    /// Racing wheel controller.<br/>Left Stick X reports the wheel rotation, Right Trigger is the acceleration pedal, and Left Trigger is the brake pedal. Includes Directional Pad and most standard buttons (A, B, X, Y, START, BACK, LB, RB). LSB and RSB are optional.
    /// </summary>
    Wheel = 0x02,
    /// <summary>
    /// Arcade stick controller.<br/>Includes a Digital Stick that reports as a DPAD (up, down, left, right), and most standard buttons (A, B, X, Y, START, BACK). The Left and Right Triggers are implemented as digital buttons and report either 0 or 0xFF. LB, LSB, RB, and RSB are optional.
    /// </summary>
    ArcadeStick = 0x03,
    /// <summary>
    /// Flight stick controller.<br/>Includes a pitch and roll stick that reports as the Left Stick, a POV Hat which reports as the Right Stick, a rudder (handle twist or rocker) that reports as Left Trigger, and a throttle control as the Right Trigger. Includes support for a primary weapon (A), secondary weapon (B), and other standard buttons (X, Y, START, BACK). LB, LSB, RB, and RSB are optional.
    /// </summary>
    FlightStick = 0x04,
    /// <summary>
    /// Dance pad controller.<br/>Includes the Directional Pad and standard buttons (A, B, X, Y) on the pad, plus BACK and START.
    /// </summary>
    DancePad = 0x05,
    /// <summary>
    /// Guitar controller.<br/>The strum bar maps to DPAD (up and down), and the frets are assigned to A (green), B (red), Y (yellow), X (blue), and LB (orange). Right Stick Y is associated with a vertical orientation sensor; Right Stick X is the whammy bar. Includes support for BACK, START, DPAD (left, right). Left Trigger (pickup selector), Right Trigger, RB, LSB (fret modifier), RSB are optional.
    /// </summary>
    Guitar = 0x06,
    /// <summary>
    /// Alternate guitar controller.<br/>Supports a larger range of movement for the vertical orientation sensor.
    /// </summary>
    GuitarAlt = 0x07,
    /// <summary>
    /// Drum controller.<br/>The drum pads are assigned to buttons: A for green (Floor Tom), B for red (Snare Drum), X for blue (Low Tom), Y for yellow (High Tom), and LB for the pedal (Bass Drum). Includes Directional-Pad, BACK, and START. RB, LSB, and RSB are optional.
    /// </summary>
    DrumKit = 0x08,
    /// <summary>
    /// Bass guitar controller.<br/>Identical to Guitar, with the distinct subtype to simplify setup.
    /// </summary>
    GuitarBass = 0x0B,
    /// <summary>
    /// Arcade pad controller.<br/>Includes Directional Pad and most standard buttons (A, B, X, Y, START, BACK, LB, RB). The Left and Right Triggers are implemented as digital buttons and report either 0 or 0xFF. Left Stick, Right Stick, LSB, and RSB are optional.
    /// </summary>
    ArcadePad = 0x13
}