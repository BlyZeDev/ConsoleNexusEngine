namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents an interface for an input condition
/// </summary>
public interface INexusInputCondition
{
    /// <summary>
    /// <see langword="true"/> if the condition is met, otherwise <see langword="false"/>
    /// </summary>
    /// <typeparam name="T">The type of the input</typeparam>
    /// <param name="toCheck">The input to check</param>
    /// <returns><see cref="bool"/></returns>
    public bool Check<T>(T toCheck) where T : struct;
}