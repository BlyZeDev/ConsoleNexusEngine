namespace ConsoleNexusEngine.Internal;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class Native
{
    [LibraryImport(Atia32, EntryPoint = "ADL_Main_Control_Create"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Main_Control_Create_32(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

    [LibraryImport(Atia32, EntryPoint = "ADL_Main_Control_Destroy"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Main_Control_Destroy_32();

    [LibraryImport(Atia32, EntryPoint = "ADL_Adapter_NumberOfAdapters_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Adapter_NumberOfAdapters_Get_32(ref int numAdapters);

    [LibraryImport(Atia32, EntryPoint = "ADL_Adapter_AdapterInfo_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Adapter_AdapterInfo_Get_32(nint info, int inputSize);

    [LibraryImport(Atia32, EntryPoint = "ADL_Overdrive5_Temperature_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Overdrive5_Temperature_Get_32(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

    [LibraryImport(Atia64, EntryPoint = "ADL_Main_Control_Create"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Main_Control_Create_64(ADL_Main_Memory_Alloc callback, int enumConnectedAdapters);

    [LibraryImport(Atia64, EntryPoint = "ADL_Main_Control_Destroy"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Main_Control_Destroy_64();

    [LibraryImport(Atia64, EntryPoint = "ADL_Adapter_NumberOfAdapters_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Adapter_NumberOfAdapters_Get_64(ref int numAdapters);

    [LibraryImport(Atia64, EntryPoint = "ADL_Adapter_AdapterInfo_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Adapter_AdapterInfo_Get_64(nint info, int inputSize);

    [LibraryImport(Atia64, EntryPoint = "ADL_Overdrive5_Temperature_Get"), UnmanagedCallConv(CallConvs = [typeof(CallConvCdecl)]), DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial int ADL_Overdrive5_Temperature_Get_64(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);

    public delegate nint ADL_Main_Memory_Alloc(int size);
}