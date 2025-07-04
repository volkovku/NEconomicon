namespace NEconomicon;

/// <summary>
/// Marks class as NEconomicon storage component.
/// </summary>
public interface IComponent<TComponent> : IComponent
    where TComponent : IComponent<TComponent>, new()
{
}

/// <summary>
/// Marks class as NEconomicon storage component.
/// </summary>
public interface IComponent
{
}
