namespace ConsoleNexusEngine;

using ConsoleNexusEngine.Common;
using System;
using System.Collections.Generic;

/// <summary>
/// Provides extension methods for <see cref="ConsoleNexusEngine"/>
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Checks if the key is found in the span
    /// </summary>
    /// <param name="keys">The span that is searched</param>
    /// <param name="key">The key to check</param>
    /// <returns><see langword="true"/> if the key was found, otherwise <see langword="false"/></returns>
    public static bool Contains(this in ReadOnlySpan<NexusKey> keys, NexusKey key)
    {
        foreach (var item in keys)
        {
            if (item == key) return true;
        }

        return false;
    }
}