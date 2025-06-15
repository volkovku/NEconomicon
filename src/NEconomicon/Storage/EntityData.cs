namespace NEconomicon.Storage;

/// <summary>
/// Represents an internal data of entity.
/// </summary>
internal sealed class EntityData
{
    private readonly HashSet<Query> _queries = new();

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
    internal readonly Dictionary<ComponentKey, IComponent> Components;

    /// <summary>
    /// A fast check components bit set.
    /// </summary>
    internal readonly BitSet ComponentsBitSet = BitSet.Empty();

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
        ComponentsBitSet.ClearAll();
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
        ComponentsBitSet.Set(compInfo.Index);
        Storage.OnComponentAdd(EntityId, this, compInfo, comp);

        isNew = true;
        return newComp;
    }

    internal bool Remove<TComponent>()
    {
        var compInfo = Storage.Scheme.GetComponentInfo(typeof(TComponent));
        if (!Components.TryGetValue(compInfo.Key, out var comp))
        {
            return false;
        }

        Storage.ReleaseComponent(comp);
        Components.Remove(compInfo.Key);
        ComponentsBitSet.Clear(compInfo.Index);
        Storage.OnComponentRemove(EntityId, this, compInfo);

        return true;
    }

    internal void AddQuery(Query query)
    {
        _queries.Add(query);
    }

    internal void RemoveQuery(Query query)
    {
        _queries.Remove(query);
    }

    private void ReleaseComponents()
    {
        foreach (var (_, comp) in Components)
        {
            Storage.ReleaseComponent(comp);
        }

        Components.Clear();
    }

    private void ReleaseQueries()
    {
        foreach (var query in _queries)
        {
            query.Remove(this);
        }

        _queries.Clear();
    }
}
