namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
internal struct AdapterInfo
{
    public int Size;
    public int AdapterIndex;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string UDID;
    public int BusNumber;
    public int DeviceNumber;
    public int FunctionNumber;
    public int VendorID;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string AdapterName;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string DisplayName;
    public int Present;
    public int Exist;
    public int DriverPath;
    public int DriverPathExt;
    public int PNPString;
    public int OSDisplayIndex;
}