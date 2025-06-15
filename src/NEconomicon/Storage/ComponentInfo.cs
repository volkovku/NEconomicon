namespace NEconomicon.Storage;

using System.Reflection;

/// <summary>
/// Provides necessary component info.
/// </summary>
public sealed class ComponentInfo
{
    private readonly ComponentField[] _fields;
    private readonly Func<IComponent> _new;
    private readonly Action<IComponent> _cleanUp;

    /// <summary>
    /// Creates info of component of specified type.
    /// </summary>
    public static ComponentInfo Create(int index, Type componentType)
    {
        if (!componentType.ImplementsInterface(typeof(IComponent<>)))
        {
            return Throw.Ex<ComponentInfo>(
                $"Can't register component of type '{componentType}',"
                + $" it should implement '{typeof(IComponent<>)}' interface.");
        }

        var componentKey = GetComponentKey(componentType);

        var newInstanceFunc = CreateNewInstanceFunc(componentType);
        var componentInstance = newInstanceFunc();
        var componentFields = EnumerateComponentFields(
            componentInstance,
            componentType);

        var flagInstance = componentFields.Length == 0
            ? Opt.Some(componentInstance)
            : Opt.None<IComponent>();

        var cleanUpFunc = CreateCleanUpFunc(componentFields);

        var componentInfo = new ComponentInfo(
            index,
            componentKey,
            componentType,
            componentFields,
            flagInstance,
            newInstanceFunc,
            cleanUpFunc);

        return componentInfo;
    }

    private ComponentInfo(
        int index,
        ComponentKey key,
        Type type,
        ComponentField[] fields,
        Opt<IComponent> flagInstance,
        Func<IComponent> fNew,
        Action<IComponent> fCleanUp)
    {
        Index = index;
        Key = key;
        Type = type;
        FlagInstance = flagInstance;
        _fields = fields;
        _new = fNew;
        _cleanUp = fCleanUp;
    }

    /// <summary>
    /// An internal index in storage scheme.
    /// </summary>
    public readonly int Index;

    /// <summary>
    /// Component serialization key.
    /// </summary>
    public readonly ComponentKey Key;

    /// <summary>
    /// Type of component which info holds there.
    /// </summary>
    public readonly Type Type;

    /// <summary>
    /// Name of component which info holds there.
    /// </summary>
    public string Name => Type.Name;

    /// <summary>
    /// Is component is a flag (doesn't have any field) contains instance of this component.
    /// </summary>
    public readonly Opt<IComponent> FlagInstance;

    /// <summary>
    /// Creates a new instance of component with hold type.
    /// </summary>
    public IComponent New() => _new();

    /// <summary>
    /// Resets component to initial state.
    /// </summary>
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

    private static Func<IComponent> CreateNewInstanceFunc(Type componentType)
    {
        var constructor = componentType.GetParamLessConstructor();
        return new Func<IComponent>(() => 
        {
            return (IComponent)constructor.Invoke(null);
        });
    }

    private static Action<IComponent> CreateCleanUpFunc(ComponentField[] fields)
    {
        return new Action<IComponent>(component =>
        {
            foreach (var field in fields)
            {
                field.ResetValueToInitial(component);
            }
        });
    }

    private static ComponentField[] EnumerateComponentFields(
        object initialValuesSource,
        Type componentType)
    {
        var componentFields = new List<ComponentField>();
        var rawFields = componentType.GetFields();
        foreach (var rawField in rawFields)
        {
            var componentField = CreateComponentField(
                initialValuesSource,
                rawField);

            componentFields.Add(componentField);
        }

        return componentFields.ToArray();
    }

    private static ComponentField CreateComponentField(
        object initialValuesSource,
        FieldInfo rawField)
    {
        var initialValue = rawField.GetValue(initialValuesSource);
        return new ComponentField(rawField, initialValue);
    }
}
