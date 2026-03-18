namespace NEconomicon.Collections;

public sealed class Aos<T>(int initialCapacity, IGrowStrategy<T> growStrategy) where T : struct
{
    private T[] _items = new T[initialCapacity];
    private int _itemsCount = 0;

    public int Count => _itemsCount;

    public int Capacity => _items.Length;

    public ref T Add()
    {
        EnsureCapacity();
        return ref _items[_itemsCount++];
    }

    public int AddValue(T value)
    {
        EnsureCapacity();
        var idx = _itemsCount++;
        _items[idx] = value;
        return idx;
    }

    public ref T GetAt(int i)=> ref _items[i];

    public void SetAt(int i, T v) => _items[i] = v;

    public void RemoveLast()
    {
        _itemsCount--;
    }

    public void Grow(int newSize)
    {
        if (_items.Length < newSize)
        {
            Array.Resize(ref _items, newSize);
        }
    }

    private void EnsureCapacity()
    {
        if (_itemsCount == _items.Length)
        {
            growStrategy.Grow(ref _items);
        }
    }
}