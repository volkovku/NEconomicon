using System.Runtime.CompilerServices;

namespace NEconomicon.Utils;

public static class Ensure
{
    public static T NotNull<T>(T? value, string name) where T : class
    {
        return value is null ? ValueCantBeNull<T>(name) : value;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static T ValueCantBeNull<T>(string name) where T : class => throw new ArgumentNullException(name);
}