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
        if (!componentType.ImplementsInterface(typeof(IComponent<>)))
        {
            return Throw.Ex<ComponentInfo>(
                $"Can't register component of type '{componentType}',"
                + $" it should implement '{typeof(IComponent<>)}' interface.");
        }

        var componentKey = GetComponentKey(componentType);
        if (_componentsByKey.TryGetValue(componentKey, out var collition))
        {
            return Throw.Ex<ComponentInfo>(
                "Component with same key already registered ("
                + $"key={componentKey},"
                + $"type={componentType})");
        }

        var newInstanceFunc = GetNewInstanceFunc(componentType);
        var cleanUpFunc = new Action<IComponent>(comp => {});
        var componentInfo = new ComponentInfo(
            componentKey, 
            newInstanceFunc, 
            cleanUpFunc
        );

        _componentsByKey.Add(componentKey, componentInfo);
        _componentsByType.Add(componentType, componentInfo);
        
        return componentInfo;
    }

    private static ComponentKey GetComponentKey(Type componentType)
    {
        var attr = componentType.GetCustomAttribute<ComponentKeyAttribute>();
        if (attr == null)
        {
            return Throw.Ex<ComponentKey>(
                $"Component '{componentType}' should be " +
                $"marked with '{nameof(ComponentKeyAttribute)}' attribute");
        }

        return attr.Key;
    }

    private static Func<IComponent> GetNewInstanceFunc(Type componentType)
    {
        var constructor = componentType.GetParamLessConstructor();
        return new Func<IComponent>(() => 
        {
            return (IComponent)constructor.Invoke(null);
        });
    }

    private static ComponentInfo ComponentNotFound(Type t)
    {
        return Throw.Ex<ComponentInfo>(
            $"Component with type '{t.Name}' not registered in scheme.");
    }
}
