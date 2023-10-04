namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using System;

/// <summary>
/// Extension methods for the Console Nexus Engine
/// </summary>
public static class NexusExtensions
{
    /// <summary>
    /// Checks if the specified key is in the span
    /// </summary>
    /// <param name="keys">The span to check</param>
    /// <param name="key">The key to check</param>
    /// <returns><see langword="true"/> if the key was found, otherwise <see langword="false"/></returns>
    public static bool Contains(this in ReadOnlySpan<NexusKey> keys, NexusKey key)
    {
        foreach (var value in keys)
        {
            if (value == key) return true;
        }

        return false;
    }
}