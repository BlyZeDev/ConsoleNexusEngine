namespace ConsoleNexusEngine.Common;

using System;
using System.Collections.Generic;

/// <summary>
/// An easy to use base class for implementing a controller
/// </summary>
public abstract class NexusController
{
    private IDictionary<NexusKey, Action> _controls;

    /// <summary>
    /// Contains all keys that invoke an action
    /// </summary>
    protected IReadOnlyDictionary<NexusKey, Action> Controls => _controls.AsReadOnly();

    /// <summary>
    /// The frames where the pressed keys should be checked and the action invoked
    /// </summary>
    protected Framerate CheckFrames { get; }

    /// <summary>
    /// Initializes the character controller
    /// </summary>
    protected NexusController(Framerate checkFrames)
    {
        _controls = new Dictionary<NexusKey, Action>();
        CheckFrames = checkFrames;
    }

    /// <summary>
    /// Registers the controls of the object
    /// </summary>
    /// <param name="controls">The controls to register</param>
    public void RegisterControls(IDictionary<NexusKey, Action> controls)
        => _controls = controls;

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
            if (Controls.TryGetValue(input.Key, out var action))
                action.Invoke();
        }
    }
}