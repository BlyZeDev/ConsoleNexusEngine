﻿namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct XINPUT_BATTERY_INFORMATION
{
    public byte BatteryType;
    public byte BatteryLevel;
}