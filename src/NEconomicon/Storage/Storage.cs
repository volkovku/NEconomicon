namespace NEconomicon.Storage;

/// <summary>
/// Represents a storage where lives data of unit.
/// </summary>
public sealed class Storage
{
    private readonly Dictionary<EntityId, EntityData> _entitiesById;
    private readonly Stack<EntityData> _entityDataPool;
    private readonly Dictionary<ComponentKey, Stack<IComponent>> _componentsPool;
    private EntityId _nextEntityId;
    
    /// <storage>
    /// Initializes a new instance of storage.
    /// </storage>
    public Storage(Scheme scheme, EntityId? nextEntityId = default)
    {
        Ensure.NotNull(scheme, nameof(scheme));

        _entitiesById = new Dictionary<EntityId, EntityData>();
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
    /// Determines is this storage empty.
    /// </summary>
    public bool IsEmpty => EntitiesCount == 0;

    /// <summary>
    /// Gets count of alive entities in this storage.
    /// </summary>
    public int EntitiesCount => _entitiesById.Count;

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    public Entity CreateEntity()
    {
        var entityId = _nextEntityId;
        _nextEntityId = _nextEntityId.Next();
        return CreateEntityInternal(entityId);
    }

    /// <summary>
    /// Creates new entity with specified symbolic identifier.
    /// If entity with same identifier already exists throws exception.
    /// </summary>
    public Entity CreateEntity(string id)
    {
        var entityId = new EntityId(id);
        return _entitiesById.ContainsKey(entityId)
            ? Throw.Ex<Entity>($"Entity with id '{id}' already exists.")
            : CreateEntityInternal(entityId);
    }

    /// <summary>
    /// Gets or creates entity with specified symbolic identifier.
    /// </summary>
    public Entity GetOrCreateEntity(string id)
    {
        return GetOrCreateEntity(id, out _);
    }

    /// <summary>
    /// Gets or creates entity with specified symbolic identifier.
    /// </summary>
    public Entity GetOrCreateEntity(string id, out bool isNew)
    {
        var entityId = new EntityId(id);
        if (_entitiesById.TryGetValue(entityId, out var entityData))
        {
            isNew = false;
            return new Entity(entityId, entityData);
        }

        isNew = true;
        return CreateEntityInternal(entityId);
    }

    /// <summary>
    /// Returns some found entity associated with specified identifier;
    /// otherwise returns none.
    /// </summary>
    public Opt<Entity> FindEntity(EntityId entityId)
    {
        if (_entitiesById.TryGetValue(entityId, out var entityData))
        {
            return Opt.Some(new Entity(entityId, entityData));
        }

        return Opt.None<Entity>();
    }

    /// <summary>
    /// Destroy specified entity.
    /// </summary>
    public bool DestroyEntity(Entity entity)
    {
        return DestroyEntity(entity.Id);
    }

    /// <summary>
    /// Destroy entity with specified identifier.
    /// </summary>
    public bool DestroyEntity(EntityId entityId)
    {
        if (!_entitiesById.TryGetValue(entityId, out var entityData))
        {
            return false;
        }

        if (!entityData.IsAlive)
        {
            return false;
        }

        entityData.Deactivate();
        _entityDataPool.Push(entityData);
        _entitiesById.Remove(entityId);

        return true;
    }

    private Entity CreateEntityInternal(EntityId entityId)
    {
        if (_entityDataPool.TryPop(out var entityData))
        {
            entityData.Reactivate(entityId);
        }
        else
        {
            entityData = new EntityData(this, entityId);
        }

        _entitiesById.Add(entityId, entityData);

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
            _componentsPool.Add(compInfo.Key, pool);
        }

        compInfo.CleanUp(comp);
        pool.Push(comp);
    }
}
