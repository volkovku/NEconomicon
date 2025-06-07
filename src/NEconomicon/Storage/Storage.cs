namespace NEconomicon.Storage;

/// <summary>
/// Represents a storage where lives data of unit.
/// </summary>
public sealed class Storage
{
    private readonly Dictionary<EntityId, EntityData> _entities;
    private readonly Stack<EntityData> _entityDataPool;
    private readonly Dictionary<ComponentKey, Stack<IComponent>> _componentsPool;
    private EntityId _nextEntityId;
    
    /// <storage>
    /// Initializes a new instance of storage.
    /// </storage>
    public Storage(Scheme scheme, EntityId? nextEntityId)
    {
        Ensure.NotNull(scheme, nameof(scheme));

        _entities = new Dictionary<EntityId, EntityData>();
        _entityDataPool = new Stack<EntityData>();
        _componentsPool = new Dictionary<ComponentKey, Stack<IComponent>>();
        _nextEntityId = nextEntityId ?? default;
        Scheme = scheme;
    }

    /// <storage>
    /// Provides a scheme of this storage.
    /// </summary>
    public readonly Scheme Scheme;

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    public Entity CreateEntity()
    {
        var entityId = _nextEntityId;
        _nextEntityId = _nextEntityId.Next();

        if (_entityDataPool.TryPop(out var entityData))
        {
            entityData.Reactivate(entityId);
        }
        else
        {
            entityData = new EntityData(this, entityId);
        }

        return new Entity(entityId, entityData);
    }

    internal (TComponent, ComponentInfo) GetComponent<TComponent>()
        where TComponent : IComponent
    {
        var compInfo = Scheme.GetComponentInfo(typeof(TComponent));
        if (!_componentsPool.TryGetValue(compInfo.Key, out var pool))
        {
            pool = new Stack<IComponent>();
        }

        TComponent comp;
        if (pool.TryPop(out var weakComp))
        {
            comp = (TComponent)weakComp;
        }
        else
        {
            comp = (TComponent)compInfo.New();
        }

        return (comp, compInfo);
    }

    internal void ReleaseComponent(IComponent comp)
    {
        var compInfo = Scheme.GetComponentInfo(comp.GetType());
        if (!_componentsPool.TryGetValue(compInfo.Key, out var pool))
        {
            pool = new Stack<IComponent>();
        }

        compInfo.CleanUp(comp);
        pool.Push(comp);
    }
}
