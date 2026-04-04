namespace NEconomicon.Model;

[AttributeUsage(AttributeTargets.Field)]
public sealed class PropertyAttribute(byte id, string? name = null) : Attribute
{
    public readonly byte Id = id;
    public readonly string? Name = name;
}