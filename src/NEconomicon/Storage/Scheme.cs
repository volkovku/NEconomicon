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

    public ComponentInfo RegisterComponent<TComponent>()
        where TComponent : IComponent<TComponent>, new()
    {
        return RegisterComponent(typeof(TComponent));
    }

    public ComponentInfo RegisterComponent(Type componentType)
    {
        var componentIndex = _componentsByKey.Count;
        var componentInfo = ComponentInfo.Create(componentIndex, componentType);
        var componentKey = componentInfo.Key;
        if (_componentsByKey.TryGetValue(componentKey, out var collision))
        {
            return Throw.Ex<ComponentInfo>(
                "Component with same key already registered ("
                + $"key={componentKey}, "
                + $"new_type={componentType}, "
                + $"old_type={collision.Type})");
        }

        _componentsByKey.Add(componentKey, componentInfo);
        _componentsByType.Add(componentType, componentInfo);
        
        return componentInfo;
    }

    private static ComponentInfo ComponentNotFound(Type t)
    {
        return Throw.Ex<ComponentInfo>(
            $"Component with type '{t.Name}' not registered in scheme.");
    }
}
