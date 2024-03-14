namespace ConsoleNexusEngine.IO;

/// <summary>
/// Represents generic interface for input conditions
/// </summary>
public interface INexusInputCondition
{
    /// <summary>
    /// <see langword="true"/> if the input meets the condition, otherwise <see langword="false"/>
    /// </summary>
    /// <param name="input">The input to check</param>
    /// <returns><see cref="bool"/></returns>
    public bool Check(INexusInput input);
}