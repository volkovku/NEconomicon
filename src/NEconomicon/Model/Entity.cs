namespace NEconomicon.Model;

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
    /// Checks if this entity has the specified component type.
    /// </summary>
    /// <typeparam name="TComponent">A component type to check.</typeparam>
    /// <returns>Returns true if entity has the component; otherwise false.</returns>
    public bool Has<TComponent>() where TComponent : Component<TComponent>, new()
    {
        return storage.HasComponent(Id, Component<TComponent>.D.Id);
    }

    /// <summary>
    /// Sets component to this entity if not exists. Otherwise, do nothing.
    /// </summary>
    /// <typeparam name="TComponent">A component to set.</typeparam>
    /// <returns>Returns true if component was set; otherwise false.</returns>
    public bool Set<TComponent>() where TComponent : Component<TComponent>, new()
    {
        return storage.AddComponent(Id, Component<TComponent>.D.Id);
    }

    /// <summary>
    /// Gets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to get its value.</param>
    /// <returns>Returns the property value; or 0 if not found.</returns>
    public long Get(Property<int> property)
    {
        return storage.TryGetPropertyValue(Id, property.ComponentId, property.Id, out var result) ? (int)result : 0;
    }

    /// <summary>
    /// Sets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to set its value.</param>
    /// <param name="value">A value to set.</param>
    public void Set(Property<int> property, int value)
    {
        storage.SetPropertyValue(Id, property.ComponentId, property.Id, value);
    }

    /// <summary>
    /// Gets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to get its value.</param>
    /// <returns>Returns the property value; or 0 if not found.</returns>
    public long Get(Property<long> property)
    {
        return storage.TryGetPropertyValue(Id, property.ComponentId, property.Id, out var result) ? result : 0;
    }

    /// <summary>
    /// Sets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to set its value.</param>
    /// <param name="value">A value to set.</param>
    public void Set(Property<long> property, long value)
    {
        storage.SetPropertyValue(Id, property.ComponentId, property.Id, value);
    }

    /// <summary>
    /// Gets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to get its value.</param>
    /// <returns>Returns the property value; or 0 if not found.</returns>
    public string Get(Property<string> property)
    {
        if (!storage.TryGetPropertyValue(Id, property.ComponentId, property.Id, out var strId))
        {
            return string.Empty;
        }

        if (strId == 0)
        {
            return string.Empty;
        }

        return storage.GetStringById((int)strId);
    }

    /// <summary>
    /// Sets the value of a property for the specified component type.
    /// </summary>
    /// <param name="property">A property to set its value.</param>
    /// <param name="value">A value to set.</param>
    public void Set(Property<string> property, string value)
    {
        var strId = storage.GetStringId(value);
        storage.SetPropertyValue(Id, property.ComponentId, property.Id, strId);
    }
}