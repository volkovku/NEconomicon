namespace NEconomicon.Exceptions;

/// <summary>
/// Represents an exception with NEconomicon specific.
/// </summary>
public sealed class NEconomiconException : Exception
{
    /// <summary>
    /// Initializes a new instance on NEconomicon exception.
    /// </summary>
    /// <param name="message">A message of exception.</param>
    public NEconomiconException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new sinatnce of NEconomicon with inner exception.
    /// </summary>
    /// <param name="message">A message of excetion.</param>
    /// <param name="innerException">An inner exception.</param>
    public NEconomiconException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}