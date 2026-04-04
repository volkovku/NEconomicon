using System.Reflection;
using NEconomicon.Exceptions;

namespace NEconomicon.Model;

/// <summary>
/// Defines component meta-data.
/// </summary>
/// <typeparam name="TComponent">A type of definied component.</typeparam>
public abstract class Component<TComponent> where TComponent : Component<TComponent>, new()
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly Exception? InitException;

    // ReSharper disable once StaticMemberInGenericType
    private static readonly TComponent? Instance;
    
    // ReSharper disable once StaticMemberInGenericType
    private static readonly ComponentDescription? Description;

    /// <summary>
    /// Gets an instance of component defintion.
    /// </summary>
    public static TComponent _ => InitException != null ? throw InitException : Instance!;

    /// <summary>
    /// Gets unified description of this component.
    /// </summary>
    public static ComponentDescription D => InitException != null ? throw InitException : Description!;

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
        foreach (var prop in cmpType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propAttr = prop.GetCustomAttribute<PropertyAttribute>();
            if (propAttr == null)
            {
                Throw.ComponentPropertyShouldBeMarkedWithPropertyAttribute(prop.Name, out InitException);
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

        InitException = null;
        Instance = instance;
        Description = new ComponentDescription(componentId, cmpAttr.Name ?? cmpType.Name, properties);
    }
}
