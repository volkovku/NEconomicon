namespace NEconomicon.Model;

/// <summary>
/// Represents a property-level access in ECS model. 
/// </summary>
/// <param name="id">A unique identifier of property on component level.</param>
/// <param name="componentId">An identifier of component which owns this property.</param>
/// <param name="name">A name of this property.</param>
/// <typeparam name="T">A type-constrains for hi-level DLS (see Entity get/set methods for example).</typeparam>
public sealed class Property<T>(byte id, ushort componentId, string name) : IProperty
{
    /// <summary>
    /// A unique identifier of property on component level.
    /// </summary>
    public byte Id { get; } = id;
    
    /// <summary>
    /// An identifier of component which owns this property.
    /// </summary>
    public ushort ComponentId { get; } = componentId;
    
    /// <summary>
    /// A name of this property.
    /// </summary>
    public string Name { get; } = name;
}
