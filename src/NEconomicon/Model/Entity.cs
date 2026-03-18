namespace NEconomicon.Model;

using NEconomicon.Storage;

/// <summary>
/// Represents an entity.
/// Entity is a union of identity and associated components. 
/// </summary>
/// <param name="id">This entity identity.</param>
/// <param name="storage">This entity data storage.</param>
public readonly struct Entity(EntityId id, EntityDataStorage storage)
{
    /// <summary>
    /// Gets this entity identity.
    /// </summary>
    public EntityId Id => id;

    /// <summary>
    /// Sets component to this entity if not exists. Otherwise do nothing.
    /// </summary>
    /// <typeparam name="TComponent">A component to set.</typeparam>
    /// <returns>Returns true if component was set; otherwise false.</returns>
    public bool Set<TComponent>()
    {
        return storage.AddComponent(Id, ComponentLookup<TComponent>.Description.Id);
    }

    public void Set<TComponent>(Func<TComponent, Property<int>> property, int value)
    {
        var componentId = ComponentLookup<TComponent>.Description.Id;
        var propertyId = property(ComponentLookup<TComponent>.Instance).Id;
        storage.SetPropertyValue(Id, componentId, propertyId, value);
    }
}