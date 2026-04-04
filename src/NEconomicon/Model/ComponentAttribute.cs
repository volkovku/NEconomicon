namespace NEconomicon.Model;

/// <summary>
/// Defines component meta data.
/// </summary>
/// <param name="id">An unique identifier of component.</param>
/// <param name="name">A name of component. If not set an original name of marked class will used.</param>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentAttribute(ushort id, string? name = null) : Attribute
{
    public readonly ushort Id = id;
    public readonly string? Name = name;
}
