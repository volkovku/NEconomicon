namespace NEconomicon;

public sealed class NEconomiconException : Exception
{
    public NEconomiconException()
    {
    }

    public NEconomiconException(string message) : base(message)
    {
    }

    public NEconomiconException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
