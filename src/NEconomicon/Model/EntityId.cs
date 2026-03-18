namespace NEconomicon.Model;

public readonly struct EntityId(uint value) : IEquatable<EntityId>
{
    public readonly uint Value = value;

    public bool Equals(EntityId otherId)
    {
        return Value == otherId.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (obj.GetType() != typeof(EntityId))
        {
            return false;
        }

        return Equals((EntityId)obj);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(EntityId left, EntityId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EntityId left, EntityId right)
    {
        return !(left == right);
    }
}
