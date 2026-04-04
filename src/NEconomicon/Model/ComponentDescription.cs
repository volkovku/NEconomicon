namespace NEconomicon.Model;

/// <summary>
/// Describes component.
/// </summary>
/// <param name="id">A unique identifier of described component.</param>
/// <param name="name">A name of described component.</param>
/// <param name="properties">A collection of component properties.</param>
public sealed class ComponentDescription(
    ushort id,
    string name,
    IReadOnlyList<IProperty> properties)
{
    /// <summary>
    /// An identifier of described component.
    /// </summary>
    public readonly ushort Id = id;

    /// <summary>
    /// A name of described component.
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// A collection of described component properties.
    /// </summary>
    public readonly IReadOnlyList<IProperty> Properties = properties;

    /// <summary>
    /// A collection of schemes where this component participates.
    /// </summary>
    public readonly ComponentSchemes Schemes = new();
}
