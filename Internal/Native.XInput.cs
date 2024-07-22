namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [DllImport("xinput1_4.dll")]
    public static extern uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);

    [DllImport("xinput1_4.dll")]
    public static extern uint XInputSetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

    [DllImport("xinput1_4.dll")]
    public static extern uint XInputGetCapabilities(uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES pCapabilites);

    [DllImport("xinput1_4.dll")]
    public static extern uint XInputGetBatteryInformation(uint dwUserIndex, byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);
}