namespace ConsoleNexusEngine.Audio;

using SoundFlow.Structs;

/// <summary>
/// Represents a playback audio device
/// </summary>
public sealed record NexusAudioDevice
{
    internal readonly DeviceInfo _deviceInfo;

    /// <summary>
    /// The name of the playback device
    /// </summary>
    public string Name => _deviceInfo.Name;

    /// <summary>
    /// <see langword="true"/> if the device is a default device, otherwise <see langword="false"/>
    /// </summary>
    public bool IsDefault => _deviceInfo.IsDefault;

    internal NexusAudioDevice(DeviceInfo deviceInfo) => _deviceInfo = deviceInfo;
}