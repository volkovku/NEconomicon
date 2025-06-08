namespace NEconomicon.Storage;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentKeyAttribute : Attribute
{
    public ComponentKeyAttribute(ulong key)
    {
        Key = new ComponentKey(key);
    }

    public readonly ComponentKey Key;
}
