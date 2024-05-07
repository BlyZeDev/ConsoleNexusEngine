namespace ConsoleNexusEngine;

/// <summary>
/// A controller for <see cref="ConsoleGame"/>
/// </summary>
/// <remarks>Provides useful input functions like a global controller</remarks>
public sealed class ConsoleController
{
    private readonly GlobalController _controller;

    internal ConsoleController() => _controller = new GlobalController();

    /// <inheritdoc cref="NexusController.AddControl(NexusInputCondition, Action)"/>
    public void AddControl(NexusInputCondition condition, Action action)
        => _controller.AddControl(condition, action);

    /// <inheritdoc cref="NexusController.RemoveControl(NexusInputCondition)"/>
    public bool RemoveControl(NexusInputCondition condition)
        => _controller.RemoveControl(condition);

    /// <summary>
    /// Invokes all registered controls
    /// </summary>
    /// <param name="inputs">The inputs from <see cref="ConsoleGame.Update"/></param>
    public void Control(in NexusInputCollection inputs) => _controller.Control(inputs);

    private sealed class GlobalController : NexusController { }
}