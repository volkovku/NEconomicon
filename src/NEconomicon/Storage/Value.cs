namespace NEconomicon.Storage;

public readonly struct Value(ulong numeric, string str, ValueKind kind)
{
    
}

/// <summary>
/// Defines supported values.
/// </summary>
public enum ValueKind : byte
{
    None = 0,
    Int32 = 1,
    String = 2,
}
