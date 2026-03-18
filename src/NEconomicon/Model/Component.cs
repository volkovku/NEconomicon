using System.Diagnostics;
using System.Reflection;
using NEconomicon.Exceptions;

namespace NEconomicon.Model;

public sealed class Component(ushort id, string name, IReadOnlyCollection<IProperty> properties)
{
    public readonly ushort Id = id;
    public readonly string Name = name;
    public readonly IReadOnlyCollection<IProperty> Properties = properties;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ComponentAttribute(ushort id, string? name = null) : Attribute
{
    public readonly ushort Id = id;
    public readonly string? Name = name;
}

public static class ComponentLookup<TComponent> where TComponent : new()
{
    internal static readonly TComponent Instance;
    internal static readonly Component Description;

    static ComponentLookup()
    {
        Instance = new TComponent();

        var cmpType = typeof(TComponent);
        var cmpAttr = cmpType.GetCustomAttribute<ComponentAttribute>();
        if (cmpAttr == null)
        {
            Throw.ComponentShouldBeMarkedWithComponentAttribute(cmpType);
            return;
        }


    }
}

public static class ComponentLookup
{
    private static readonly 
}

public sealed class Property<T>(byte id, string name, Component component) : IProperty
{
    public byte Id => id;
    public string Name => name;
    public Component Component => component;
}

public interface IProperty
{
    byte Id { get; }
    string Name { get; }
    Component Component { get; }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAttribute(byte id, string? name = null) : Attribute
{
    public readonly byte Id = id;
    public readonly string? Name = name;
}