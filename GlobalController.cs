namespace ConsoleNexusEngine;

/// <summary>
/// A controller for <see cref="ConsoleGame"/>
/// </summary>
/// <remarks>Provides a global controller</remarks>
public sealed class GlobalController
{
    private readonly Dictionary<INexusInputCondition, Action> _controls;

    internal GlobalController() => _controls = [];

    /// <summary>
    /// Adds a control to the <see cref="GlobalController"/>
    /// </summary>
    /// <remarks>
    /// <see langword="false"/> if the condition already existed, otherwise <see langword="true"/>
    /// </remarks>
    /// <param name="condition">The condition that has to be met to execute the action</param>
    /// <param name="action">The action to execute if the condition is met</param>
    /// <returns><see cref="bool"/></returns>
    public bool AddControl(INexusInputCondition condition, Action action) => _controls.TryAdd(condition, action);

    /// <summary>
    /// Removes a control from the <see cref="GlobalController"/>
    /// </summary>
    /// <remarks>
    /// <see langword="true"/> if the condition was found and removed, otherwise <see langword="false"/>
    /// </remarks>
    /// <param name="condition">The condition to remove</param>
    /// <returns><see cref="bool"/></returns>
    public bool RemoveControl(INexusInputCondition condition) => _controls.Remove(condition);

    /// <summary>
    /// Invokes all registered controls of the <see cref="GlobalController"/>
    /// </summary>
    /// <param name="inputs">The inputs from <see cref="ConsoleGame.Update(in NexusInputCollection)"/></param>
    public void Control(in NexusInputCollection inputs)
    {
        foreach (var control in _controls)
        {
            if (control.Key.Check(inputs.MousePosition))
            {
                control.Value();
                continue;
            }

            foreach (var input in inputs.Keys.AsSpan())
            {
                if (control.Key.Check(input))
                {
                    control.Value();
                    break;
                }
            }
        }
    }
}