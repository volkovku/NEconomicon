namespace NEconomicon.Storage;

public sealed class ComponentInfo
{
    private readonly Func<IComponent> _new;
    private readonly Action<IComponent> _cleanUp;

    public static ComponentInfo Create(Type componentType)
    {
        if (!componentType.ImplementsInterface(typeof(IComponent<>)))
        {
            return Throw.Ex<ComponentInfo>(
                $"Can't register component of type '{componentType}',"
                + $" it should implement '{typeof(IComponent<>)}' interface.");
        }

        var componentKey = GetComponentKey(componentType);
        var newInstanceFunc = GetNewInstanceFunc(componentType);
        var cleanUpFunc = new Action<IComponent>(comp => {});

        var componentInfo = new ComponentInfo(
            componentKey,
            componentType,
            newInstanceFunc,
            cleanUpFunc);

        return componentInfo;
    }

    public ComponentInfo(
        ComponentKey key,
        Type type,
        Func<IComponent> fNew,
        Action<IComponent> fCleanUp)
    {
        Key = key;
        Type = type;
        _new = fNew;
        _cleanUp = fCleanUp;
    }

    public readonly ComponentKey Key;

    public readonly Type Type;

    public string Name => Type.Name;
    
    public IComponent New() => _new();

    public void CleanUp(IComponent component) => _cleanUp(component);

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

}
