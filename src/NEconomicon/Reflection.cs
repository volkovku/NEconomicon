namespace NEconomicon;

using System.Reflection;

public static class Reflection
{
    public static TAttr? GetCustomAttribute<TAttr>(this Type type)
        where TAttr : Attribute
    {
        return (TAttr?)Attribute.GetCustomAttribute(type, typeof(TAttr));
    }

    public static ConstructorInfo GetParamLessConstructor(
        this Type type,
        bool includePrivate = true)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includePrivate)
        {
            flags = flags | BindingFlags.NonPublic;
        }

        var c = type.GetConstructor(flags, null, Type.EmptyTypes, null);
        if (c == null)
        {
            return Throw.Ex<ConstructorInfo>(
                "Paramter less constructor not found: (" +
                $"type={type})");
        }

        return c;
    }

    public static bool ImplementsInterface<TInterface>(this Type type)
    {
        return ImplementsInterface(type, typeof(TInterface));
    }

    public static bool ImplementsInterface(this Type type, Type iType)
    {
        if (!iType.IsInterface)
        {
            return false;
        }

        if (iType.IsGenericType)
        {
            return type.GetInterfaces().Any(i =>
                    i.IsGenericType
                    && i.GetGenericTypeDefinition() == iType);
        }

        if (iType.IsGenericTypeDefinition)
        {
            return type.GetInterfaces().Any(i =>
                    i.IsGenericTypeDefinition
                    && i == iType);
        }

        return iType.IsAssignableFrom(type);
    }
}
