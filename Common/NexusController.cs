namespace ConsoleNexusEngine.Common;

using System;
using System.Collections.Generic;

/// <summary>
/// An easy to use base class for implementing a controller
/// </summary>
public abstract class NexusController
{
    /// <summary>
    /// Contains all keys that invoke an action
    /// </summary>
    protected readonly IReadOnlyDictionary<NexusKey, Action> _controller;

    /// <summary>
    /// The frames where the pressed keys should be checked and the action invoked
    /// </summary>
    protected Framerate CheckFrames { get; }

    /// <summary>
    /// Initializes the character controller
    /// </summary>
    protected NexusController(IDictionary<NexusKey, Action> controller, Framerate checkFrames)
    {
        _controller = controller.AsReadOnly();
        CheckFrames = checkFrames;
    }

    /// <summary>
    /// This should be called in the <see cref="ConsoleGame.Update(in ReadOnlySpan{INexusInput})"/> method
    /// </summary>
    /// <param name="framerate">The framerate of the game</param>
    /// <param name="inputs">The inputs that were made</param>
    public void Control(Framerate framerate, in ReadOnlySpan<INexusInput> inputs)
    {
        if (framerate % CheckFrames is not 0) return;

        foreach (var input in inputs)
        {
            if (_controller.TryGetValue(input.Key, out var action))
                action.Invoke();
        }
    }
}