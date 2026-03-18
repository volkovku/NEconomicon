using NEconomicon.Utils;

namespace NEconomicon.Collections;

public sealed class IncGrowStrategy<T>(int growSize) : IGrowStrategy<T>
{
    public void Grow(ref T[] items)
    {
        Ensure.NotNull(items, nameof(items));
        Array.Resize(ref items, items.Length + growSize);
    }
}

public interface IGrowStrategy<T>
{
    void Grow(ref T[] items);
}