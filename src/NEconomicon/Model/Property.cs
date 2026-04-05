using System.Runtime.CompilerServices;
using NEconomicon.Exceptions;

namespace NEconomicon.Model;

/// <summary>
/// Represents a property-level access in ECS model. 
/// </summary>
/// <param name="id">A unique identifier of property on component level.</param>
/// <param name="componentId">An identifier of component which owns this property.</param>
/// <param name="name">A name of this property.</param>
/// <typeparam name="T">A type-constrains for hi-level DLS (see Entity get/set methods for example).</typeparam>
public sealed class Property<T>(byte id, string name, ushort componentId, string componentName) : IProperty
{
    private readonly ComponentSchemes _schemes = new();

    /// <summary>
    /// A unique identifier of property on component level.
    /// </summary>
    public byte Id { get; } = id;

    /// <summary>
    /// A name of this property.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// An identifier of component which owns this property.
    /// </summary>
    public ushort ComponentId { get; } = componentId;

    /// <summary>
    /// A name of component which owns this property.
    /// </summary>
    public string ComponentName { get; } = componentName;

    /// <summary>
    /// Gets this property value type.
    /// </summary>
    public PropertyValueType ValueType { get; } = GetValueType();

    /// <summary>
    /// Adds scheme where component of this property are participates.
    /// </summary>
    /// <param name="scheme">A scheme to add.</param>
    public void AddScheme(Scheme scheme) => _schemes.AddScheme(scheme);

    /// <summary>
    /// Determines is this property defined in specified scheme.
    /// </summary>
    /// <param name="scheme">A scheme to check.</param>
    public bool DefinedInScheme(Scheme scheme) => _schemes.ContainsScheme(scheme);

    private static PropertyValueType GetValueType()
    {
        if (typeof(T) == typeof(int))
        {
            return PropertyValueType.Int32;
        }

        if (typeof(T) == typeof(long))
        {
            return PropertyValueType.Int64;
        }

        if (typeof(T) == typeof(float))
        {
            return PropertyValueType.Float32;
        }

        if (typeof(T) == typeof(double))
        {
            return PropertyValueType.Float64;
        }

        if (typeof(T) == typeof(string))
        {
            return PropertyValueType.String;
        }

        if (typeof(T) == typeof(DateTime))
        {
            return PropertyValueType.DateTime;
        }

        if (typeof(T) == typeof(TimeSpan))
        {
            return PropertyValueType.TimeSpan;
        }

        return NotSupportedValueType();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static PropertyValueType NotSupportedValueType()
    {
        throw new NEconomiconException($"Not supported property value type (type={typeof(T)})");
    }
}
