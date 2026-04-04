namespace NEconomicon.Model;

/// <summary>
/// Describes component property.
/// </summary>
public interface IProperty
{
    /// <summary>
    /// An identifier of property. It unique in component scope.
    /// </summary>
    byte Id { get; }
    
    /// <summary>
    /// A name of property.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Adds scheme where component of this property are participates.
    /// </summary>
    /// <param name="scheme">A scheme to add.</param>
    internal void AddScheme(Scheme scheme);
    
    /// <summary>
    /// Determines is this property defined in specified scheme.
    /// </summary>
    /// <param name="scheme">A scheme to check.</param>
    internal bool DefinedInScheme(Scheme scheme);
}
