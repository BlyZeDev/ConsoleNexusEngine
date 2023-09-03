namespace ConsoleNexusEngine;

using System;
using System.Collections.Generic;

/// <summary>
/// Provides extension methods for <see cref="ConsoleNexusEngine"/>
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Checks if the value is found in the span
    /// </summary>
    /// <typeparam name="T"><typeparamref name="T"/> is not <see langword="null"/></typeparam>
    /// <param name="span">The span that is searched</param>
    /// <param name="value">The value to check</param>
    /// <returns><see langword="true"/> if the value was found, otherwise <see langword="false"/></returns>
    public static bool Contains<T>(this in ReadOnlySpan<T> span, T value) where T : notnull
    {
        foreach (var item in span)
        {
            if (EqualityComparer<T>.Default.Equals(item, value))
                return true;
        }

        return false;
    }
}