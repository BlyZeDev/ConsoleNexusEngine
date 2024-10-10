namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal interface IGpuMonitor : IMonitor
{
    public static IMonitor GetMonitor()
    {
        try
        {
            _ = Native.NvmlInitV2();

            return new NvidiaGpuMonitor();
        }
        catch (DllNotFoundException) { }

        try
        {
            _ = Native.Is64BitOS
                ? Native.ADL_Main_Control_Create_64(Marshal.AllocCoTaskMem, 1)
                : Native.ADL_Main_Control_Create_32(Marshal.AllocCoTaskMem, 1);

            return new AmdGpuMonitor();
        }
        catch (DllNotFoundException) { }

        return new EmptyMonitor();
    }
}