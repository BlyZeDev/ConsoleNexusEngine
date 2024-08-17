namespace ConsoleNexusEngine.Internal;

using System.Runtime.InteropServices;

internal sealed class AmdGpuMonitor : IGpuMonitor
{
    private const int TEMP_INDEX = 0;

    private readonly int _adapterIndex;
    private readonly string _modelName;

    private readonly ADL_Overdrive5_Temperature_Get _getTemperature;

    public AmdGpuMonitor()
    {
        _ = Native.Is64BitOS
            ? Native.ADL_Main_Control_Create_64(Marshal.AllocCoTaskMem, 1)
            : Native.ADL_Main_Control_Create_32(Marshal.AllocCoTaskMem, 1);

        var numberOfAdapters = 0;
        _ = Native.Is64BitOS
            ? Native.ADL_Adapter_NumberOfAdapters_Get_64(ref numberOfAdapters)
            : Native.ADL_Adapter_NumberOfAdapters_Get_32(ref numberOfAdapters);

        var adapterInfoData = new AdapterInfo[numberOfAdapters];
        var adapterBuffer = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AdapterInfo)) * numberOfAdapters);
        Marshal.StructureToPtr(adapterInfoData, adapterBuffer, false);

        _ = Native.Is64BitOS
            ? Native.ADL_Adapter_AdapterInfo_Get_64(adapterBuffer, Marshal.SizeOf(typeof(AdapterInfo)) * numberOfAdapters)
            : Native.ADL_Adapter_AdapterInfo_Get_32(adapterBuffer, Marshal.SizeOf(typeof(AdapterInfo)) * numberOfAdapters);

        adapterInfoData = (AdapterInfo[])Marshal.PtrToStructure(adapterBuffer, typeof(AdapterInfo[]))!;

        _adapterIndex = adapterInfoData[0].AdapterIndex;
        _modelName = adapterInfoData[0].DisplayName;

        _getTemperature = Native.Is64BitOS ? Native.ADL_Overdrive5_Temperature_Get_64 : Native.ADL_Overdrive5_Temperature_Get_32;
    }

    public string GetModel() => _modelName;

    public int GetTemperature()
    {
        var temperature = new ADLTemperature();
        temperature.Size = Marshal.SizeOf(temperature);

        _getTemperature(_adapterIndex, TEMP_INDEX, ref temperature);

        return temperature.Temperature / 1000;
    }

    public void Shutdown() => _ = Native.Is64BitOS
        ? Native.ADL_Main_Control_Destroy_64() : Native.ADL_Main_Control_Destroy_32();

    private delegate int ADL_Overdrive5_Temperature_Get(int adapterIndex, int thermalControllerIndex, ref ADLTemperature temperature);
}