namespace NEconomicon.Storage;

/// <summary>
/// Represents a query which track entities correspond to specified constraints.
/// </summary>
public sealed class Query
{
    private readonly Storage _storage;
    private readonly BitSet _include;
    private readonly BitSet _exclude;
    private readonly HashSet<EntityData> _entities;

    internal Query(Storage storage, BitSet include, BitSet exclude)
    {
        _storage = storage;
        _include = include;
        _exclude = exclude;
        _entities = new HashSet<EntityData>();
    }

    /// <summary>
    /// Returns count of entities in this query.
    /// </summary>
    public int Count => _entities.Count;

    /// <summary>
    /// Returns collection of entities.
    /// Be aware it's allocates new collection under the hood.
    /// </summary>
    public IReadOnlyList<Entity> GetEntities()
    {
        var entities = new List<Entity>();
        CollectEntitiesTo(entities);
        return entities;
    }

    /// <summary>
    /// Collects query entities to specified collection.
    /// </summary>
    public void CollectEntitiesTo(ICollection<Entity> destination)
    {
        foreach (var entityData in _entities)
        {
            destination.Add(new Entity(entityData.EntityId, entityData));
        }
    }

    internal void Touch(EntityData entity)
    {
        if (IsMatch(entity))
        {
            if (_entities.Add(entity))
            {
                entity.AddQuery(this);
            }

            return;
        }

        if (_entities.Remove(entity))
        {
            entity.RemoveQuery(this);
        }
    }

    internal void Remove(EntityData entity)
    {
        _entities.Remove(entity);
    }

    private bool IsMatch(EntityData entity)
    {
        return entity.ComponentsBitSet.CheckAllFrom(_include)
            && !entity.ComponentsBitSet.CheckAnyFrom(_exclude);
    }
}
