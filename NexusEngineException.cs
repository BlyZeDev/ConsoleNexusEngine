namespace ConsoleNexusEngine;

/// <summary>
/// An exception thrown if something went wrong with the Engine
/// </summary>
[Serializable]
public sealed class NexusEngineException : Exception
{
	internal NexusEngineException(string message) : base(message) { }

	internal NexusEngineException(string message, Exception inner) : base(message, inner) { }
}