namespace ConsoleNexusEngine.Internal;

using System.Text;

internal sealed class NvidiaGpuMonitor : IGpuMonitor
{
    private const int NVML_TEMPERATURE_GPU = 0;

    private readonly nint _device;

    public NvidiaGpuMonitor()
    {
        _ = Native.NvmlInitV2();
        _ = Native.NvmlDeviceGetHandleByIndexV2(0, out var device);

        _device = device;
    }

    public unsafe string GetModel()
    {
        const int Length = 128;

        var name = stackalloc byte[Length];
        _ = Native.NvmlDeviceGetName(_device, name, Length);

        return Encoding.ASCII.GetString(new ReadOnlySpan<byte>(name, Length)).TrimEnd(char.MinValue);
    }

    public int GetTemperature()
    {
        _ = Native.NvmlDeviceGetTemperature(_device, NVML_TEMPERATURE_GPU, out var temperature);

        return (int)temperature;
    }

    public void Shutdown() => _ = Native.NvmlShutdown();
}