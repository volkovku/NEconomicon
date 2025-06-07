namespace NEconomicon.Storage;

using System.Collections.Generic;

public sealed class Scheme
{
    private readonly Dictionary<Type, ComponentInfo> _componentsByType
        = new Dictionary<Type, ComponentInfo>();

    private readonly Dictionary<ComponentKey, ComponentInfo> _componentsByKey
        = new Dictionary<ComponentKey, ComponentInfo>();

    public ComponentInfo GetComponentInfo<TComponent>()
        where TComponent : IComponent
    {
        return GetComponentInfo(typeof(TComponent));
    }

    public ComponentInfo GetComponentInfo(Type componentType)
    {
        if (!_componentsByType.TryGetValue(componentType, out var componentInfo))
        {
            return ComponentNotFound(componentType);
        }

        return componentInfo;
    }

    private static ComponentInfo ComponentNotFound(Type t)
    {
        throw new InvalidOperationException(
            $"Component with type '{t.Name}' not registered in scheme.");
    }
}
