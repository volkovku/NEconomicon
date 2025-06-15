namespace NEconomicon.Storage;

public sealed class QueryKey : IEquatable<QueryKey>
{
    public readonly BitSet Include;
    public readonly BitSet Exclude;

    public QueryKey(BitSet include, BitSet exclude)
    {
        Include = include;
        Exclude = exclude;
    }

    public bool IsMatch(BitSet bitSet)
    {
        return bitSet.CheckAnyFrom(Include)
            && !bitSet.CheckAnyFrom(Exclude);
    }

    public bool Equals(QueryKey? other)
    {
        return Include == other!.Include
            && Exclude == other!.Exclude;
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return Equals((QueryKey)other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = (int) 2166136261;
            hash = (hash * 16777619) ^ Include.GetHashCode();
            hash = (hash * 16777619) ^ Exclude.GetHashCode();
            return hash;
        }
    }
}
