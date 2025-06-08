namespace NEconomicon.Storage;

/// <summary>
/// A strong type representation of component key.
/// </summary>
public readonly struct ComponentKey : IEquatable<ComponentKey>
{
    /// <summary>
    /// Initializes a new instance of component key.
    /// </summary>
    public ComponentKey(ulong value)
    {
        Value = value;
    }

    /// <summary>
    /// A numeric representation of this component key.
    /// </summary>
    public readonly ulong Value;

    public bool Equals(ComponentKey other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return Equals((ComponentKey)other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"CompKey({Value})";
    }

    public static bool operator ==(ComponentKey a, ComponentKey b)
    {
        return a.Value == b.Value;
    }

    public static bool operator !=(ComponentKey a, ComponentKey b)
    {
        return a.Value != b.Value;
    }

}
