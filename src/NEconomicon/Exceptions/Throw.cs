namespace NEconomicon.Exceptions;

using System.Runtime.CompilerServices;
using Model;

public static class Throw
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void EntityNotFound(EntityId entityId) => EntityNotFound<object>(entityId);

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static T EntityNotFound<T>(EntityId entityId)
    {
        throw new NEconomiconException($"Entity not found (entity_id={entityId.Value})");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ComponentShouldBeMarkedWithComponentAttribute(Type componentType, out Exception exception)
    {
        exception = new NEconomiconException(
            $"Component '{componentType.FullName}' should be marked with " +
            $"'{typeof(ComponentAttribute).FullName}' attribute");

        throw exception;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void ComponentPropertyShouldBeMarkedWithPropertyAttribute(string propertyName, out Exception exception)
    {
        exception = new NEconomiconException(
            $"Component property '{propertyName}' should be marked with " +
            $"'{typeof(PropertyAttribute).FullName}' attribute");

        throw exception;
    }
}