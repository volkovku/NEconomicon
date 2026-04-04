using NEconomicon.Exceptions;

namespace NEconomicon.Model;

/// <summary>
/// Represents components scheme.
/// </summary>
public sealed class Scheme
{
    private readonly Dictionary<int, ComponentDescription> _components = new();

    /// <summary>
    /// Gets count of components in this scheme.
    /// </summary>
    public int ComponentsCount => _components.Count;

    /// <summary>
    /// Gets components registered in this scheme.
    /// </summary>
    public IEnumerable<ComponentDescription> Components => _components.Values;
    
    /// <summary>
    /// Registers component in this scheme.
    /// </summary>
    /// <typeparam name="TComponent">A type of component to register.</typeparam>
    public void Register<TComponent>() where TComponent : Component<TComponent>, new()
    {
        var description = Component<TComponent>.D;
        var id = description.Id;
        if (!_components.TryAdd(id, description))
        {
            throw new NEconomiconException($"Component already registered (component={typeof(TComponent)})");
        }

        description.Schemes.AddScheme(this);
        foreach (var prop in description.Properties)
        {
            prop.AddScheme(this);
        }
    }
}
