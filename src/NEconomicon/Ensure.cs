namespace NEconomicon;

using System;

/// <summary>
/// Provides a set of methods to ensure that code works well.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Ensures that specified value is not null.
    /// </summary>
    public static T NotNull<T>(T value, string name) where T : class
    {
        return ReferenceEquals(null, value)
            ? Throw.ArgNull<T>(name)
            : value;
    }
}
