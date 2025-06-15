namespace NEconomicon.Storage;

public sealed class QueryBuilder :
    IQueryBuilderInclude,
    IQueryBuilderExclude
{
    private readonly Storage _storage;
    private readonly BitSet _include = BitSet.Empty();
    private readonly BitSet _exclude = BitSet.Empty();

    internal QueryBuilder(Storage storage)
    {
        _storage = storage;
    }

    public IQueryBuilderInclude Include<TComponent>()
        where TComponent : IComponent<TComponent>, new()
    {
        var compInfo = _storage.Scheme.GetComponentInfo(typeof(TComponent));
        _include.Set(compInfo.Index);
        return this;
    }

    public IQueryBuilderExclude Exclude<TComponent>()
        where TComponent : IComponent<TComponent>, new()
    {
        var compInfo = _storage.Scheme.GetComponentInfo(typeof(TComponent));
        if (_include.Check(compInfo.Index))
        {
            return Throw.Ex<IQueryBuilderExclude>(
                $"Can't add '{typeof(TComponent).Name}' component. "
                + "It already defined in include section.");
        }

        _exclude.Set(compInfo.Index);
        return this;
    }

    public Query Build()
    {
        return _storage.RegisterQuery(_include, _exclude);
    }
}

public readonly struct QueryBuilderEntryPoint
{
    private readonly Storage _storage;

    public QueryBuilderEntryPoint(Storage storage)
    {
        _storage = storage;
    }

    public IQueryBuilderInclude Include<TComponent>()
        where TComponent : IComponent<TComponent>, new()
    {
        return new QueryBuilder(_storage).Include<TComponent>();
    }

    public IQueryBuilderExclude Exclude<TComponent>()
        where TComponent : IComponent<TComponent>, new()
    {
        return new QueryBuilder(_storage).Exclude<TComponent>();
    }
}

public interface IQueryBuilder
{
    Query Build();
}

public interface IQueryBuilderInclude : IQueryBuilder
{
    IQueryBuilderInclude Include<TComponent>()
        where TComponent : IComponent<TComponent>, new();

    IQueryBuilderExclude Exclude<TComponent>()
        where TComponent : IComponent<TComponent>, new();
}

public interface IQueryBuilderExclude : IQueryBuilder
{
    IQueryBuilderExclude Exclude<TComponent>()
        where TComponent : IComponent<TComponent>, new();
}
