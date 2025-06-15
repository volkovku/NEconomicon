namespace NEconomicon.Storage;

internal sealed class QueryCollection
{
    private readonly Storage _storage;
    private readonly BitSet _trackedComponents;
    private readonly Dictionary<QueryKey, Query> _queries;

    internal QueryCollection(Storage storage)
    {
        _storage = storage;
        _trackedComponents = BitSet.Empty();
        _queries = new Dictionary<QueryKey, Query>();
    }

    internal Query RegisterQuery(QueryKey queryKey)
    {
        if (_queries.TryGetValue(queryKey, out var query))
        {
            return query;
        }

        _trackedComponents.SetAllFrom(queryKey.Include);
        _trackedComponents.SetAllFrom(queryKey.Exclude);

        query = new Query(_storage, queryKey.Include, queryKey.Exclude);
        _queries.Add(queryKey, query);

        foreach (var entity in _storage.EnumerateEntitiesInternal())
        {
            query.Touch(entity);
        }

        return query;
    }

    internal void OnComponentAdd(
        EntityId entityId,
        EntityData entityData,
        ComponentInfo compInfo,
        IComponent component)
    {
        OnEntityComponentsSetChanged(compInfo, entityData);
    }

    internal void OnComponentRemove(
        EntityId entityId,
        EntityData entityData,
        ComponentInfo compInfo)
    {
        OnEntityComponentsSetChanged(compInfo, entityData);
    }

    private void OnEntityComponentsSetChanged(
        ComponentInfo compInfo,
        EntityData entityData)
    {
        if (!_trackedComponents.Check(compInfo.Index))
        {
            return;
        }

        foreach (var query in _queries.Values)
        {
            query.Touch(entityData);
        }
    }
}
