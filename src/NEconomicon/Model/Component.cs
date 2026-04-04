using System.Reflection;
using NEconomicon.Exceptions;

namespace NEconomicon.Model;

/// <summary>
/// Defines component meta-data.
/// </summary>
/// <typeparam name="TComponent">A type of defined component.</typeparam>
public abstract class Component<TComponent> where TComponent : Component<TComponent>, new()
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Exception? InitException;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly TComponent? Instance;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ComponentDescription? Description;

    /// <summary>
    /// Gets an instance of component definition.
    /// </summary>
#pragma warning disable CA1000
#pragma warning disable CA1707
    public static TComponent _ => InitException != null ? throw InitException : Instance!;
#pragma warning restore CA1707
#pragma warning restore CA1000

    /// <summary>
    /// Gets unified description of this component.
    /// </summary>
#pragma warning disable CA1000
    public static ComponentDescription D => InitException != null ? throw InitException : Description!;
#pragma warning restore CA1000

    static Component()
    {
        var cmpType = typeof(TComponent);
        var cmpAttr = cmpType.GetCustomAttribute<ComponentAttribute>();
        if (cmpAttr == null)
        {
            Throw.ComponentShouldBeMarkedWithComponentAttribute(cmpType, out InitException);
            return;
        }

        var componentId = cmpAttr.Id;
        var instance = Activator.CreateInstance<TComponent>();
        var properties = new List<IProperty>();
        foreach (var field in cmpType.GetFields(BindingFlags.Public | BindingFlags.Instance))
        {
            var propAttr = field.GetCustomAttribute<PropertyAttribute>();
            if (propAttr == null)
            {
                Throw.ComponentPropertyShouldBeMarkedWithPropertyAttribute(field.Name, out InitException);
                return;
            }

            var propertyTypeArg = field.FieldType.GenericTypeArguments[0];
            var propertyType = typeof(Property<>).MakeGenericType(propertyTypeArg);
            var propertyValue = Activator.CreateInstance(
                propertyType,
                propAttr.Id,
                componentId,
                propAttr.Name ?? field.Name
            );

            field.SetValue(instance, propertyValue);
            properties.Add((IProperty)propertyValue!);
        }

        InitException = null;
        Instance = instance;
        Description = new ComponentDescription(componentId, cmpAttr.Name ?? cmpType.Name, properties);
    }
}
