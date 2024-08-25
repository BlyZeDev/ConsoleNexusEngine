namespace ConsoleNexusEngine;

using BackgroundTimer;
using System.Runtime.InteropServices;

/// <summary>
/// Provides useful system monitoring functions for <see cref="ConsoleGame"/>
/// </summary>
public sealed class NexusSystemMonitor
{
    private const int ByteToMegabyte = 1000000;

    private const uint QueryInformation = 0x400;
    private const uint VmRead = 0x10;

    private readonly BackgroundTimer _updateTimer;
    private readonly IMonitor _cpuMonitor;
    private readonly IMonitor _gpuMonitor;

    internal bool IsEnabled { get; set; }

    /// <summary>
    /// The windows version the process in running on
    /// </summary>
    public string WindowsVersion => Native.WindowsVersion;

    /// <summary>
    /// <see langword="true"/> if the operating system is x64, otherwise <see langword="false"/>
    /// </summary>
    public bool Is64BitOS => Native.Is64BitOS;

    /// <summary>
    /// How much memory this process uses at the moment, in megabytes
    /// </summary>
    /// <remarks>
    /// This value is updated every second
    /// </remarks>
    public int AllocatedMemory { get; private set; }

    /// <summary>
    /// How much memory this process used at it's peak, in megabytes
    /// </summary>
    /// /// <remarks>
    /// This value is updated every second
    /// </remarks>
    public int PeakAllocatedMemory { get; private set; }

    /// <summary>
    /// The name of the CPU
    /// </summary>
    public string CpuModel { get; }

    /// <summary>
    /// The name of the GPU
    /// </summary>
    public string GpuModel { get; }

    /// <summary>
    /// The current CPU temperature in celsius
    /// </summary>
    /// <remarks>
    /// This value is updated every second
    /// </remarks>
    public int CpuTemperature { get; private set; }

    /// <summary>
    /// The current GPU temperature in celsius
    /// </summary>
    /// <remarks>
    /// This value is updated every second
    /// </remarks>
    public int GpuTemperature { get; private set; }

    internal NexusSystemMonitor()
    {
        _updateTimer = BackgroundTimer.StartNew(TimeSpan.FromSeconds(1), Update);
        _cpuMonitor = ICpuMonitor.GetMonitor();
        _gpuMonitor = IGpuMonitor.GetMonitor();
        
        CpuModel = _cpuMonitor.GetModel();
        GpuModel = _gpuMonitor.GetModel();
    }

    private void Update(int ticks)
    {
        if (!IsEnabled) return;

        var processHandle = Native.OpenProcess(QueryInformation | VmRead, false, (int)Native.GetCurrentProcessId());
        if (processHandle == nint.Zero) return;
        
        try
        {
            Native.GetProcessMemoryInfo(processHandle, out var counters, (uint)Marshal.SizeOf<PROCESS_MEMORY_COUNTERS>());

            AllocatedMemory = (int)(counters.WorkingSetSize / ByteToMegabyte);
            PeakAllocatedMemory = (int)(counters.PeakWorkingSetSize / ByteToMegabyte);
        }
        finally
        {
            Native.CloseHandle(processHandle);
        }

        CpuTemperature = _cpuMonitor.GetTemperature();
        GpuTemperature = _gpuMonitor.GetTemperature();
    }

    internal void Dispose() => _updateTimer.Dispose();
}