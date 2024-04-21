namespace ConsoleNexusEngine.IO;

/// <summary>
/// Base class for implementing a controller
/// </summary>
public abstract class NexusController
{
    /// <summary>
    /// The controls of the controller
    /// </summary>
    public Dictionary<NexusInputCondition, Action> Controls { get; set; }

    /// <summary>
    /// Initializes a new controller
    /// </summary>
    protected NexusController() => Controls = [];

    /// <summary>
    /// Invokes all actions of the inputs that are made
    /// </summary>
    /// <param name="inputs">The inputs that were made</param>
    public void Control(in NexusInputCollection inputs)
    {
        foreach (var (condition, action) in Controls)
        {
            if (condition._isMousePosCondition)
            {
                if (condition.Check(inputs.MousePosition)) action();
            }
            else
            {
                foreach (var key in inputs.Keys)
                {
                    if (condition.Check(key)) action();
                }
            }
        }
    }
}