using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;
using NEconomicon.Exceptions;

namespace NEconomicon.Model;

public abstract class Component<TComponent> where TComponent : Component<TComponent>, new()
{
    private static readonly Exception? _initException;
    private static readonly TComponent? _instance;
    private static readonly ComponentDescription? _description;

    static Component()
    {
        var cmpType = typeof(TComponent);
        var cmpAttr = cmpType.GetCustomAttribute<ComponentAttribute>();
        if (cmpAttr == null)
        {
            Throw.ComponentShouldBeMarkedWithComponentAttribute(cmpType, out _initException);
            return;
        }

        var componentId = cmpAttr.Id;
        var instance = Activator.CreateInstance<TComponent>()!;
        var properties = new List<IProperty>();
        foreach (var prop in cmpType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propAttr = prop.GetCustomAttribute<PropertyAttribute>();
            if (propAttr == null)
            {
                Throw.ComponentPropertyShouldBeMarkedWithPropertyAttribute(prop.Name, out _initException);
                return;
            }

            var propertyTypeArg = prop.PropertyType.GenericTypeArguments[0];
            var propertyType = typeof(Property<>).MakeGenericType(propertyTypeArg);
            var propertyValue = Activator.CreateInstance(
                propertyType,
                propAttr.Id,
                componentId,
                propAttr.Name ?? prop.Name
            );

            prop.SetValue(instance, propertyValue);
            properties.Add((IProperty)propertyValue!);
        }

        _initException = null;
        _instance = instance;
        _description = new ComponentDescription(componentId, cmpAttr.Name ?? cmpType.Name, properties);
    }

    public static TComponent _
    {
        get
        {
            if (_initException != null)
            {
                throw _initException;
            }

            return _instance!;
        }
    }

    public static ComponentDescription D
    {
        get
        {
            if (_initException != null)
            {
                throw _initException;
            }

            return _description!;
        }
    }
}

public sealed class ComponentDescription(ushort id, string name, IReadOnlyCollection<IProperty> properties)
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

public static class ComponentLookup<TComponent>
{
    private static readonly Exception? _initException;
    private static readonly TComponent? _instance;
    private static readonly ComponentDescription? _description;

    public static TComponent Instance
    {
        get
        {
            if (_initException != null)
            {
                throw _initException;
            }

            return _instance!;
        }
    }

    public static ComponentDescription Description
    {
        get
        {
            if (_initException != null)
            {
                throw _initException;
            }

            return _description!;
        }
    }

    static ComponentLookup()
    {
        var cmpType = typeof(TComponent);
        var cmpAttr = cmpType.GetCustomAttribute<ComponentAttribute>();
        if (cmpAttr == null)
        {
            Throw.ComponentShouldBeMarkedWithComponentAttribute(cmpType, out _initException);
            return;
        }

        var instance = Activator.CreateInstance<TComponent>()!;
        var properties = new List<IProperty>();
        foreach (var prop in cmpType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propAttr = prop.GetCustomAttribute<PropertyAttribute>();
            if (propAttr == null)
            {
                Throw.ComponentPropertyShouldBeMarkedWithPropertyAttribute(prop.Name, out _initException);
                return;
            }

            var propertyTypeArg = prop.PropertyType.GenericTypeArguments[0];
            var propertyType = typeof(Property<>).MakeGenericType(propertyTypeArg);
            var propertyValue = Activator.CreateInstance(propertyType, propAttr.Id, propAttr.Name ?? prop.Name);
            prop.SetValue(instance, propertyValue);
            properties.Add((IProperty)propertyValue!);
        }

        _initException = null;
        _description = new ComponentDescription(cmpAttr.Id, cmpAttr.Name ?? cmpType.Name, properties);
        _instance = instance;
    }
}

public sealed class Property<T>(byte id, ushort componentId, string name) : IProperty
{
    public byte Id { get; } = id;
    public ushort ComponentId { get; } = componentId;
    public string Name { get; } = name;
}

public interface IProperty
{
    byte Id { get; }
    string Name { get; }
}

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAttribute(byte id, string? name = null) : Attribute
{
    public readonly byte Id = id;
    public readonly string? Name = name;
}