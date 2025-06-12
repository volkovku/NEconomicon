namespace NEconomicon.Storage;

/// <summary>
/// Represents an internal data of entity.
/// </summary>
internal sealed class EntityData
{
    /// <summary>
    /// A scheme of registered components.
    /// </summary>
    internal readonly Storage Storage;

    /// <summary>
    /// An identifier of entity which data holds in this instance.
    /// </summary>
    internal EntityId EntityId;

    /// <summary>
    /// A collection of components associated with this entity.
    /// </summary>
    Dictionary<ComponentKey, IComponent> Components;

    /// <summary>
    /// Current count of components in this entity.
    /// </summary>
    internal int ComponentsCount;

    /// <summary>
    /// Determines is this entity is alive.
    /// </summary>
    internal bool IsAlive;

    /// <summary>
    /// Initializes a new instance of entity data.
    /// </summary>
    internal EntityData(Storage storage, EntityId entityId)
    {
        Storage = storage;
        EntityId = entityId;
        Components = new Dictionary<ComponentKey, IComponent>();
        ComponentsCount = 0;
        IsAlive = true;
    }
    
    /// <summary>
    /// Initializes this entity data.
    /// </summary>
    internal void Reactivate(EntityId entityId)
    {
        EntityId = entityId;
        IsAlive = true;
    }

    /// <summary>
    /// Deactivates current entity.
    /// </summary>
    internal void Deactivate()
    {
        ReleaseComponents();
        IsAlive = false;
    }

    /// <summary>
    /// Returns found component of specified type; otherwise none.
    /// </summary>
    internal Opt<TComponent> Find<TComponent>() where TComponent : IComponent
    {
        var compInf = Storage.Scheme.GetComponentInfo(typeof(TComponent));
        var compKey = compInf.Key;
        if (Components.TryGetValue(compKey, out var comp))
        {
            return Opt.Some((TComponent)comp);
        }

        return Opt.None<TComponent>();
    }

    /// <summary>
    /// Gets exists component or adds new.
    /// </summary>
    internal TComponent GetOrAdd<TComponent>(out bool isNew)
        where TComponent : IComponent
    {
        if (Find<TComponent>().TryGet(out var comp))
        {
            isNew = false;
            return comp;
        }

        var (newComp, compInfo) = Storage.GetComponent<TComponent>();
        Components.Add(compInfo.Key, newComp);

        isNew = true;
        return newComp;
    }

    private void ReleaseComponents()
    {
        foreach (var (_, comp) in Components)
        {
            Storage.ReleaseComponent(comp);
        }

        Components.Clear();
    }
}
