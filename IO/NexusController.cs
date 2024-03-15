namespace ConsoleNexusEngine.IO;

/// <summary>
/// Base class for implementing a controller
/// </summary>
public abstract class NexusController
{
    /// <summary>
    /// The controls of the controller
    /// </summary>
    public Dictionary<INexusInputCondition, Action> Controls { get; set; }

    /// <summary>
    /// Initializes a new controller
    /// </summary>
    protected NexusController() => Controls = [];

    /// <summary>
    /// Invokes all actions of the inputs that are made
    /// </summary>
    /// <param name="inputs">The inputs that were made</param>
    public void Control(in ReadOnlySpan<INexusInput> inputs)
    {
        if (inputs.IsEmpty) return;

        foreach (var input in inputs)
        {
            foreach (var control in Controls)
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