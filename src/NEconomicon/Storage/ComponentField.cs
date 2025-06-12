namespace NEconomicon.Storage;

using System.Reflection;

/// <summary>
/// Represents field of component.
/// </summary>
public sealed class ComponentField
{
    /// <summary>
    /// Initializes a new instance of ComponentField class.
    /// </summary>
    public ComponentField(FieldInfo rawField, object? initialValue)
    {
        Name = rawField.Name;
        ValueType = rawField.FieldType;
        InitialValue = initialValue;
        RawField = rawField;
    }

    /// <summary>
    /// A name of field.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// A type of field value.
    /// </summary>
    public readonly Type ValueType;

    /// <summary>
    /// An initial value of field.
    /// </summary>
    public readonly object? InitialValue;

    /// <summary>
    /// A reference to original wrapped field.
    /// </summary>
    public readonly FieldInfo RawField;

    /// <summary>
    /// Resents component field value to initial value.
    /// </summary>
    public void ResetValueToInitial(IComponent component)
    {
        RawField.SetValue(component, InitialValue);
    }
}
