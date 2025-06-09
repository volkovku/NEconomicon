namespace NEconomicon.Storage;

using System.Globalization;

/// <summary>
/// Represents an Entity identifier.
/// </summary>
public readonly struct EntityId : IEquatable<EntityId>
{
    private readonly string? _symbolicValue;
    private readonly uint _numericValue;

    /// <summary>
    /// Initializes a new instance of EntityId.
    /// </summary>
    public EntityId(uint value)
    {
        _numericValue = value;
        _symbolicValue = null;
    }

    /// <summary>
    /// Initializes a new instance of EntityId with symbolic representation.
    /// </summary>
    public EntityId(string value)
    {
        _numericValue = 0;
        _symbolicValue = Ensure
            .NotBlank(value, nameof(value))
            .ToLower(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Determines that this identifier represent as symbolic value.
    /// </summary>
    public bool IsSymbolic => _symbolicValue != null;

    /// <summary>
    /// Determines that this identifier represent as numeric value.
    /// </summary>
    public bool IsNumeric => !IsSymbolic;
 
    /// <summary>
    /// Returns next identifier in sequence.
    /// </summary>
    public EntityId Next()
    {
        if (IsSymbolic)
        {
            return Throw.Ex<EntityId>(
                "Can't calc next identifier for symbolic representation.");
        }

        return new EntityId(_numericValue + 1);
    }

    public bool Equals(EntityId other)
    {
        return _symbolicValue == other._symbolicValue
            && _numericValue == other._numericValue;
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
        if (IsSymbolic)
        {
            return _symbolicValue!.GetHashCode();
        }

        return _numericValue.GetHashCode();
    }

    public override string ToString()
    {
        if (IsSymbolic)
        {
            return $"EntityId('{_symbolicValue}')";
        }
        
        return $"EntityId({_numericValue})";
    }

    public static bool operator ==(EntityId a, EntityId b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(EntityId a, EntityId b)
    {
        return !a.Equals(b);
    }
}
