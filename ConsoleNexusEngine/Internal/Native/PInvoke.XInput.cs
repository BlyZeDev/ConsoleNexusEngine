namespace ConsoleNexusEngine.Internal.Native;

using System.Runtime.InteropServices;

internal static partial class PInvoke
{
    [LibraryImport(XInput1_4, SetLastError = true)]
    public static partial uint XInputGetState(uint dwUserIndex, ref XINPUT_STATE pState);

    [LibraryImport(XInput1_4, SetLastError = true)]
    public static partial uint XInputSetState(uint dwUserIndex, ref XINPUT_VIBRATION pVibration);

    [LibraryImport(XInput1_4, SetLastError = true)]
    public static partial uint XInputGetCapabilities(uint dwUserIndex, uint dwFlags, ref XINPUT_CAPABILITIES pCapabilites);

    [LibraryImport(XInput1_4, SetLastError = true)]
    public static partial uint XInputGetBatteryInformation(uint dwUserIndex, byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);
}