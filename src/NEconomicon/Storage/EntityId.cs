namespace NEconomicon.Storage;

using System.Runtime.InteropServices;

/// <summary>
/// Represents an Entity identifier.
/// </summary>
public readonly struct EntityId : IEquatable<EntityId>
{
    /// <summary>
    /// Initializes a new instance of EntityId.
    /// values.
    /// </summary>
    public EntityId(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// An identifier of entity.
    /// </summary>
    public readonly uint Value;

    /// <summary>
    /// Returns next identifier in sequence.
    /// </summary>
    public EntityId Next()
    {
        return new EntityId(Value + 1);
    }

    public bool Equals(EntityId other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return Equals((EntityId)other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"EntityId({Value})";
    }

    public static bool operator ==(EntityId a, EntityId b)
    {
        return a.Value == b.Value;
    }

    public static bool operator !=(EntityId a, EntityId b)
    {
        return a.Value != b.Value;
    }
}
