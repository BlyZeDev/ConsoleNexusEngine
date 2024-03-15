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
    /// <see langword="true"/> if <paramref name="key"/> is currently pressed, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="key">The key to check for</param>
    /// <returns><see cref="bool"/></returns>
    public bool IsKeyPressed(NexusKey key)
        => (Native.GetAsyncKeyState((int)key) & 0x8000) is not 0;

    /// <summary>
    /// Simulates mouse and keyboard inputs
    /// </summary>
    /// <param name="inputs">The inputs to simulate</param>
    public void SimulateInput(in ReadOnlySpan<INexusInput> inputs) => _console.WriteInput(inputs);

    /// <inheritdoc cref="SimulateInput(in ReadOnlySpan{INexusInput})"/>
    public void SimulateInput(params INexusInput[] inputs) => SimulateInput(inputs.AsSpan());

    /// <summary>
    /// Adds a control to the global controller
    /// </summary>
    /// <param name="condition">The condition that has to be met</param>
    /// <param name="action">The action that should be invoked</param>
    public void AddControl(INexusInputCondition condition, Action action)
        => _controller.Controls.Add(condition, action);

    /// <summary>
    /// Adds multiple controls to the global controller
    /// </summary>
    /// <param name="controls">The controls to add</param>
    public void AddControls(IDictionary<INexusInputCondition, Action> controls)
    {
        foreach (var control in controls) AddControl(control.Key, control.Value);
    }

    /// <summary>
    /// Invokes all registered controls
    /// </summary>
    /// <param name="inputs">The inputs from <see cref="ConsoleGame.Update(in ReadOnlySpan{INexusInput})"/></param>
    public void Control(in ReadOnlySpan<INexusInput> inputs) => _controller.Control(inputs);

    private sealed class GlobalController : NexusController { }
}