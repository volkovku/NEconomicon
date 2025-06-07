namespace NEconomicon;

using System.Runtime.CompilerServices;

/// <summary>
/// Provides a set of methods to throw exceptions.
/// </summary>
public static class Throw
{
    /// <summary>
    /// Throws NEconomiconException with specified message.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void Ex(string msg) => throw new NEconomiconException(msg);

    /// <summary>
    /// Throws NEconomiconException with specified message.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T Ex<T>(string msg) => throw new NEconomiconException(msg);

    /// <summary>
    /// Throws ArgumentNullException with specified argument name.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ArgNull(string arg) => throw new ArgumentNullException(arg);

    /// <summary>
    /// Throws ArgumentNullException with specified argument name.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T ArgNull<T>(string arg) => throw new ArgumentNullException(arg);
}
