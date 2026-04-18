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
    /// Gets component by its identifier.
    /// If component not found throws exception.
    /// </summary>
    /// <param name="componentId">An identifier of component to get.</param>
    /// <returns>Returns component.</returns>
    public ComponentDescription GetComponent(int componentId) => _components[componentId];
    
    /// <summary>
    /// Registers component in this scheme.
    /// </summary>
    /// <typeparam name="TComponent">A type of component to register.</typeparam>
    public void Register<TComponent>() where TComponent : Component<TComponent>, new()
    {
        var component = Component<TComponent>.D;
        var id = component.Id;
        if (!_components.TryAdd(id, component))
        {
            throw new NEconomiconException($"Component already registered (component={typeof(TComponent)})");
        }

        component.Schemes.AddScheme(this);
        foreach (var prop in component.Properties)
        {
            prop.AddScheme(this);
        }
    }
}
