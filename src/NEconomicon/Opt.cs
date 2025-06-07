namespace NEconomicon;

using System.Collections;

/// <summary>
/// Represents an optional value which may exists or may not.
/// It's similar with Nullable, but can handle reference types.
/// </summary>
public readonly struct Opt<T> : 
    IEquatable<Opt<T>>,
    IEnumerable<T>,
    IEnumerable
{
    private readonly T _value;

    /// <summary>
    /// Determines is value exists or not.
    /// </summary>
    public readonly bool HasValue;

    /// <summary>
    /// Represents not exists value.
    /// </summary>
    public static readonly Opt<T> None = new Opt<T>();

    /// <summary>
    /// Initializes a new instance of Opt<T> struct with specified value.
    /// </summary>
    public Opt(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary>
    /// Returns value if exists; otherwise throws exception.
    /// </summary>
    public T Get()
    {
        return HasValue ? _value : Throw.Ex<T>("Value not exists.");
    }

    /// <summary>
    /// Gets exists value or returns specified default value instead.
    /// </summary>
    public T GetOr(T defaultValue)
    {
        return HasValue ? _value : defaultValue;
    }

    /// <summary>
    /// Gets exists value or returns calculated value instead.
    /// </summary>
    public T GetOrCalc(Func<T> f)
    {
        return HasValue ? _value : f();
    }

    /// <summary>
    /// Returns true is value exists; otherwise false.
    /// If value exists sets it to out arg.
    /// </summary>
    public bool TryGet(out T value)
    {
        if (HasValue)
        {
            value = _value;
            return true;
        }

        value = default!;
        return false;
    }

    /// <summary>
    /// Maps current exists value to another.
    /// </summary>
    public Opt<R> Map<R>(Func<T, R> map)
    {
        return HasValue ? new Opt<R>(map(_value)) : Opt<R>.None;
    }

    /// <summary>
    /// Maps current exists value to another.
    /// </summary>
    public Opt<R> FlatMap<R>(Func<T, Opt<R>> map)
    {
        return HasValue ? map(_value) : Opt<R>.None;
    }

    /// <summary>
    /// Returns true if this and other opt values are equals.
    /// </summary>
    public bool Equals(Opt<T> other)
    {
        if (!HasValue && !other.HasValue)
        {
            return true;
        }

        if (HasValue || other.HasValue)
        {
            return false;
        }

        return Equals(_value, other._value);
    }

    /// <summary>
    /// Returns true if this and other values are equals.
    /// </summary>
    public override bool Equals(object? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return other is Opt<T> otherOpt && Equals(otherOpt);
    }

    /// <summary>
    /// Returns hash code of this opt value.
    /// </summary>
    public override int GetHashCode()
    {
        if (!HasValue)
        {
            return 0;
        }

        if (ReferenceEquals(null, _value))
        {
            return 0;
        }

        return _value.GetHashCode();
    }

    /// <summary>
    /// Returns string representation of this opt value.
    public override string ToString()
    {
        if (!HasValue)
        {
            return "None()";
        }

        if (ReferenceEquals(null, _value))
        {
            return "Some(null)";
        }

        return $"Some({_value})";
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (HasValue)
        {
            yield return _value;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)this.GetEnumerator();
    }
}

/// <summary>
/// Provides a set of helpers to work with Opt<T> struct.
/// </summary>
public static class Opt
{
    /// <summary>
    /// Returns a some value.
    /// </summary>
    public static Opt<T> Some<T>(T value) => new Opt<T>(value);

    /// <summary>
    /// Returns none.
    /// </summary>
    public static Opt<T> None<T>() => Opt<T>.None;

}
