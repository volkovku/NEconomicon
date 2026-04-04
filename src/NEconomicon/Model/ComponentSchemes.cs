namespace NEconomicon.Model;

/// <summary>
/// Represents a collection of schemes where component participates.
/// </summary>
public sealed class ComponentSchemes
{
    private const int FastCheckSlotsCount = 5;

    private int _schemesCount;
    private Scheme? _scheme0;
    private Scheme? _scheme1;
    private Scheme? _scheme2;
    private Scheme? _scheme3;
    private Scheme? _scheme4;
    private List<Scheme>? _schemesN;

    /// <summary>
    /// Returns count of registered schemes.
    /// </summary>
    public int Count => _schemesCount;
    
    /// <summary>
    /// Adds specified scheme to this collection.
    /// </summary>
    /// <param name="scheme">A scheme to add.</param>
    public void AddScheme(Scheme scheme)
    {
        if (ContainsScheme(scheme))
        {
            return;
        }

        switch (_schemesCount)
        {
            case 0: _scheme0 = scheme; break;
            case 1: _scheme1 = scheme; break;
            case 2: _scheme2 = scheme; break;
            case 3: _scheme3 = scheme; break;
            case 4: _scheme4 = scheme; break;
            default:
                _schemesN ??= [];
                _schemesN.Add(scheme);
                break;
        }

        _schemesCount++;
    }

    /// <summary>
    /// Checks is specified scheme exists in this collection.
    /// </summary>
    /// <param name="scheme">A scheme to check.</param>
    /// <returns>Returns true if specified scheme exists in this collection; otherwise returns false.</returns>
    public bool ContainsScheme(Scheme scheme) =>
        _scheme0 == scheme ||
        _scheme1 == scheme ||
        _scheme2 == scheme ||
        _scheme3 == scheme ||
        _scheme4 == scheme ||
        (_schemesCount > FastCheckSlotsCount && _schemesN!.Contains(scheme));
}
