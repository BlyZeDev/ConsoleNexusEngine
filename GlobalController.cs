namespace ConsoleNexusEngine;

/// <summary>
/// A controller for <see cref="ConsoleGame"/>
/// </summary>
/// <remarks>Provides a global controller</remarks>
public sealed class GlobalController
{
    private readonly InternalGlobalController _controller;

    internal GlobalController() => _controller = new InternalGlobalController();

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
    public void Update(in NexusInputCollection inputs) => _controller.Control(inputs);

    private sealed class InternalGlobalController : NexusController { }
}