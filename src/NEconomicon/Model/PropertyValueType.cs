namespace NEconomicon.Model;

/// <summary>
/// Provides a set of possible property value types.
/// </summary>
public enum PropertyValueType : byte
{
    Unknown = 0,
    Int32 = 1,
    Int64 = 2,
    Float32 = 3,
    Float64 = 4,
    String = 5,
    DateTime = 6,
    TimeSpan = 7
}
