namespace ConsoleNexusEngine.Internal;

using SoundFlow.Components;
using SoundFlow.Structs;

internal sealed record PlayingSoundInfo
{
    public required DeviceInfo DeviceInfo { get; init; }
    public required SoundPlayer Player { get; init; }
}