namespace ConsoleNexusEngine;

using System;

/// <summary>
/// A controller for <see cref="ConsoleGame"/>
/// </summary>
/// <remarks>Provides useful input themed functions like a global controller</remarks>
public sealed class ConsoleController
{
    private readonly CmdConsole _console;
    private readonly GlobalController _controller;

    internal ConsoleController(CmdConsole console)
    {
        _console = console;
        _controller = new GlobalController();
    }

    /// <summary>
    /// Adds a control to the global controller
    /// </summary>
    /// <param name="condition">The condition that has to be met</param>
    /// <param name="action">The action that should be invoked</param>
    public void AddControl(NexusInputCondition condition, Action action)
        => _controller.Controls.Add(condition, action);

    /// <summary>
    /// Adds multiple controls to the global controller
    /// </summary>
    /// <param name="controls">The controls to add</param>
    public void AddControls(IDictionary<NexusInputCondition, Action> controls)
    {
        foreach (var control in controls) AddControl(control.Key, control.Value);
    }

    /// <summary>
    /// Invokes all registered controls
    /// </summary>
    /// <param name="inputs">The inputs from <see cref="ConsoleGame.Update"/></param>
    public void Control(in NexusInputCollection inputs) => _controller.Control(inputs);

    private sealed class GlobalController : NexusController { }
}