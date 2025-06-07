namespace NEconomicon.Storage;

public sealed class ComponentInfo
{
    private readonly Func<IComponent> _new;
    private readonly Action<IComponent> _cleanUp;

    public ComponentInfo(
        ComponentKey key,
        Func<IComponent> fNew,
        Action<IComponent> fCleanUp)
    {
        Key = key;
        _new = fNew;
        _cleanUp = fCleanUp;
    }

    public readonly ComponentKey Key;

    public IComponent New() => _new();

    public void CleanUp(IComponent component) => _cleanUp(component);
}
