namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport(Nvml, EntryPoint = "nvmlInit_v2"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int NvmlInitV2();

    [LibraryImport(Nvml, EntryPoint = "nvmlDeviceGetHandleByIndex_v2"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int NvmlDeviceGetHandleByIndexV2(uint index, out nint device);

    [LibraryImport(Nvml, EntryPoint = "nvmlDeviceGetName"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static unsafe partial int NvmlDeviceGetName(nint device, byte* name, int length);

    [LibraryImport(Nvml, EntryPoint = "nvmlDeviceGetTemperature"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int NvmlDeviceGetTemperature(nint device, int sensorType, out uint temp);

    [LibraryImport(Nvml, EntryPoint = "nvmlShutdown"), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int NvmlShutdown();
}