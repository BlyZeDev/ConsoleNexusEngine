namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;

public abstract partial class ConsoleGame
{
    private static readonly NexusKey[] _keys;

    static ConsoleGame()
    {
        _keys = ((NexusKey[])Enum.GetValues(typeof(NexusKey)));
    }

    private static bool IsKeyPressed(NexusKey key)
        => Native.IsKeyPressed(key);

    private static ReadOnlySpan<NexusKey> GetPressedKeys()
    {
        var currentlyPressedKeys = new SpanBuilder<NexusKey>();
        
        foreach (var key in _keys)
        {
            if (IsKeyPressed(key))
                currentlyPressedKeys.Append(key);
        }

        return currentlyPressedKeys.AsReadOnlySpan();
    }
}