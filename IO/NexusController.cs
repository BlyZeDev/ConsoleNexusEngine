namespace ConsoleNexusEngine.IO;

/// <summary>
/// Base class for implementing a controller
/// </summary>
public abstract class NexusController
{
    private readonly SortedDictionary<NexusInputCondition, Action> _controls;

    private SortedDictionary<NexusInputCondition, Action>.Enumerator enumerator;
    private int mousePosConditions;

    /// <summary>
    /// The controls of this controller
    /// </summary>
    protected IReadOnlyDictionary<NexusInputCondition, Action> Controls => _controls.AsReadOnly();

    /// <summary>
    /// Initializes a new controller
    /// </summary>
    protected NexusController()
    {
        _controls = new SortedDictionary<NexusInputCondition, Action>(new NexusInputConditionComparer());
        mousePosConditions = 0;
    }

    /// <summary>
    /// Adds a control
    /// </summary>
    /// <param name="condition">The condition the invoke the action</param>
    /// <param name="action">The action to invoke if the condition is met</param>
    public void AddControl(NexusInputCondition condition, Action action)
    {
        _controls.Add(condition, action);

        if (condition._isMousePosCondition) mousePosConditions++;
    }

    /// <summary>
    /// Removes a control
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if the control could be removed, otherwise <see langword="false"/>
    /// </remarks>
    /// <param name="condition">The condition to removed</param>
    /// <returns><see cref="bool"/></returns>
    public bool RemoveControl(NexusInputCondition condition)
    {
        if (_controls.Remove(condition))
        {
            if (condition._isMousePosCondition) mousePosConditions--;

            return true;
        }

        return false;
    }

    /// <summary>
    /// Invokes all actions of the inputs that are made
    /// </summary>
    /// <param name="inputs">The inputs that were made</param>
    public void Control(in NexusInputCollection inputs)
    {
        enumerator = _controls.GetEnumerator();

        for (int i = 0; i < mousePosConditions; i++)
        {
            enumerator.MoveNext();

            if (enumerator.Current.Key.Check(inputs.MousePosition)) enumerator.Current.Value();
        }

        while (enumerator.MoveNext())
        {
            foreach (var key in inputs.Keys)
            {
                if (enumerator.Current.Key.Check(key)) enumerator.Current.Value();
            }
        }
    }

    private readonly struct NexusInputConditionComparer : IComparer<NexusInputCondition>
    {
        public readonly int Compare(NexusInputCondition? x, NexusInputCondition? y)
        {
            ArgumentNullException.ThrowIfNull(x);
            ArgumentNullException.ThrowIfNull(y);

            return x._isMousePosCondition ? (y._isMousePosCondition ? 0 : -1) : 1;
        }
    }
}