namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using ConsoleNexusEngine.Internal;
using System;

public abstract partial class ConsoleGame
{
    private static readonly GameInputKey[] _keys;

    static ConsoleGame()
    {
        _keys = (GameInputKey[])Enum.GetValues(typeof(GameInputKey));
    }

    private static ReadOnlySpan<GameInputKey> GetPressedKeys()
    {
        var currentlyPressedKeys = new SpanBuilder<GameInputKey>();

        foreach (var key in _keys)
        {
            if (Native.IsKeyPressed(key))
                currentlyPressedKeys.Append(key);
        }

        return currentlyPressedKeys.AsReadOnlySpan();
    }
}