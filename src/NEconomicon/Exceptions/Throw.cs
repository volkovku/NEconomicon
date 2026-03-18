namespace NEconomicon.Exceptions;

using System.Runtime.CompilerServices;
using NEconomicon.Model;

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
    public static void ComponentShouldBeMarkedWithComponentAttribute(Type componentType)
    {
        throw new NEconomiconException(
            $"Component '{componentType.FullName}' should be marked with " +
            $"'{typeof(ComponentAttribute).FullName}' attribute");
    }
}