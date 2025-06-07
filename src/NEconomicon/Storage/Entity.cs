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
    /// Determines is this entity is alive.
    /// </summary>
    public bool IsAlive()
    {
        return _data.IsAlive && _data.EntityId == Id;
    }

    /// <summary>
    /// Returns some component if found, otherwise none.
    /// </summary>
    public Opt<TComponent> Find<TComponent>() where TComponent : IComponent
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

