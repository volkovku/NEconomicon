namespace NEconomicon.Storage;

/// <summary>
/// Represents an entity.
/// </summary>
public struct Entity
{
    private readonly EntityData _data;

    /// <summary>
    /// Initializes a new instance of entity.
    /// </summary>
    internal Entity(EntityId id, EntityData data)
    {
        Id = id;
        _data = Ensure.NotNull(data, nameof(data));
    }

    /// <summary>
    /// An identifier of this entity.
    /// </summary>
    public readonly EntityId Id;

    /// <summary>
    /// Returns count of components associated with this entity.
    /// </summary>
    public int ComponentsCount => _data.ComponentsCount;

    /// <summary>
    /// Determines is this entity is alive.
    /// </summary>
    public bool IsAlive()
    {
        return _data.IsAlive && _data.EntityId == Id;
    }
    /// <summary>
    /// Gets exists component or adds new.
    /// </summary>
    public TComponent GetOrAdd<TComponent>()
        where TComponent : IComponent<TComponent>
    {
        return GetOrAdd<TComponent>(out _);
    }

    /// <summary>
    /// Gets exists component or adds new.
    /// </summary>
    public TComponent GetOrAdd<TComponent>(out bool isNew)
        where TComponent : IComponent<TComponent>
    {
        EnsureIsAlive(nameof(GetOrAdd));
        return _data.GetOrAdd<TComponent>(out isNew);
    }

    /// <summary>
    /// Returns some component if found, otherwise none.
    /// </summary>
    public Opt<TComponent> Find<TComponent>()
        where TComponent : IComponent<TComponent>
    {
        EnsureIsAlive(nameof(Find));
        return _data.Find<TComponent>();
    }

    private void EnsureIsAlive(string context)
    {
        if (IsAlive())
        {
            return;
        }

        Throw.Ex($"Can't perform '{context}', entity is not alive");
    }

}

