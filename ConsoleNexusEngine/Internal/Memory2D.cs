﻿namespace ConsoleNexusEngine.Internal;

using System;

internal readonly struct Memory2D<T> where T : struct
{
    private readonly Memory<T> _memory;

    public readonly Span<T> Span => _memory.Span;

    public readonly int Width { get; }
    public readonly int Height { get; }

    public readonly int Length => _memory.Length;

    public Memory2D(in Memory<T> memory, in int width, in int height)
    {
        _memory = memory;
        Width = width;
        Height = height;
    }

    public Memory2D(in int width, in int height) : this(new T[width * height], width, height) { }

    public readonly ref T this[in int i] => ref _memory.Span[i];

    public readonly ref T this[in int x, in int y] => ref _memory.Span[MathHelper.GetIndex(x, y, Width)];

    public readonly Memory2D<T> Resize(in int width, in int height)
    {
        var newMemory = new T[width * height].AsMemory();

        _memory.TryCopyTo(newMemory);

        return new Memory2D<T>(newMemory, width, height);
    }

    public readonly ReadOnlyMemory2D<T> ToReadOnly() => new(_memory, Width, Height);
}