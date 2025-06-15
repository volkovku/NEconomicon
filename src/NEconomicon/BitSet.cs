namespace NEconomicon;

public sealed class BitSet : IEquatable<BitSet>
{
    private const int LongBitsCount = 64;
    private const int F1Upper = LongBitsCount;
    private const int F2Upper = F1Upper + LongBitsCount;
    private const int F3Upper = F2Upper + LongBitsCount;
    private const int F4Upper = F3Upper + LongBitsCount;
    private const int F5Upper = F4Upper + LongBitsCount;
    private const int F6Upper = F5Upper + LongBitsCount;
    private const int F7Upper = F6Upper + LongBitsCount;
    private const int F8Upper = F7Upper + LongBitsCount;

    public const int IndexesCount = F8Upper;
    public const int MinIndex = 0;
    public const int MaxIndex = IndexesCount - 1;

    private ulong _f1;
    private ulong _f2;
    private ulong _f3;
    private ulong _f4;
    private ulong _f5;
    private ulong _f6;
    private ulong _f7;
    private ulong _f8;

    public static BitSet Empty()
    {
        return new BitSet(0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL, 0UL);
    }

    public static BitSet Full()
    {
        return new BitSet(
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue,
            ulong.MaxValue);
    }

    public BitSet(ulong f1, ulong f2, ulong f3, ulong f4, ulong f5, ulong f6, ulong f7, ulong f8)
    {
        _f1 = f1;
        _f2 = f2;
        _f3 = f3;
        _f4 = f4;
        _f5 = f5;
        _f6 = f6;
        _f7 = f7;
        _f8 = f8;
    }

    public void Set(int index)
    {
        if (index < F1Upper)
        {
            _f1 |= 1UL << index;
        }
        else if (index < F2Upper)
        {
            _f2 |= 1UL << (index - F1Upper);
        }
        else if (index < F3Upper)
        {
            _f3 |= 1UL << (index - F2Upper);
        }
        else if (index < F4Upper)
        {
            _f4 |= 1UL << (index - F3Upper);
        }
        else if (index < F5Upper)
        {
            _f5 |= 1UL << (index - F4Upper);
        }
        else if (index < F6Upper)
        {
            _f6 |= 1UL << (index - F5Upper);
        }
        else if (index < F7Upper)
        {
            _f7 |= 1UL << (index - F6Upper);
        }
        else if (index < F8Upper)
        {
            _f8 |= 1UL << (index - F7Upper);
        }
        else
        {
            Throw.Ex("Too big index to set: " + index);
        }
    }

    public void SetAllFrom(BitSet bitSet)
    {
        _f1 |= bitSet._f1;
        _f2 |= bitSet._f2;
        _f3 |= bitSet._f3;
        _f4 |= bitSet._f4;
        _f5 |= bitSet._f5;
        _f6 |= bitSet._f6;
        _f7 |= bitSet._f7;
        _f8 |= bitSet._f8;
    }

    public void ClearAll()
    {
        _f1 = 0UL;
        _f2 = 0UL;
        _f3 = 0UL;
        _f4 = 0UL;
        _f5 = 0UL;
        _f6 = 0UL;
        _f7 = 0UL;
        _f8 = 0UL;
    }

    public void Clear(int index)
    {
        if (index < F1Upper)
        {
            _f1 &= ~(1UL << index);
        }
        else if (index < F2Upper)
        {
            _f2 &= ~(1UL << (index - F1Upper));
        }
        else if (index < F3Upper)
        {
            _f3 &= ~(1UL << (index - F2Upper));
        }
        else if (index < F4Upper)
        {
            _f4 &= ~(1UL << (index - F3Upper));
        }
        else if (index < F5Upper)
        {
            _f5 &= ~(1UL << (index - F4Upper));
        }
        else if (index < F6Upper)
        {
            _f6 &= ~(1UL << (index - F5Upper));
        }
        else if (index < F7Upper)
        {
            _f7 &= ~(1UL << (index - F6Upper));
        }
        else if (index < F8Upper)
        {
            _f8 &= ~(1UL << (index - F7Upper));
        }
        else
        {
            Throw.Ex("Too big index to clear: " + index);
        }
    }

    public bool Check(int index)
    {
        if (index < F1Upper)
        {
            return (_f1 & (1UL << index)) != 0UL;
        }
        else if (index < F2Upper)
        {
            return (_f2 & (1UL << (index - F1Upper))) != 0UL;
        }
        else if (index < F3Upper)
        {
            return (_f3 & (1UL << (index - F2Upper))) != 0UL;
        }
        else if (index < F4Upper)
        {
            return (_f4 & (1UL << (index - F3Upper))) != 0UL;
        }
        else if (index < F5Upper)
        {
            return (_f5 & (1UL << (index - F4Upper))) != 0UL;
        }
        else if (index < F6Upper)
        {
            return (_f6 & (1UL << (index - F5Upper))) != 0UL;
        }
        else if (index < F7Upper)
        {
            return (_f7 & (1UL << (index - F6Upper))) != 0UL;
        }
        else if (index < F8Upper)
        {
            return (_f8 & (1UL << (index - F7Upper))) != 0UL;
        }
        else
        {
            return Throw.Ex<bool>("Too big index to check: " + index);
        }
    }

    public bool CheckAllFrom(BitSet other)
    {
        return (_f1 & other._f1) == other._f1
            && (_f2 & other._f2) == other._f2
            && (_f3 & other._f3) == other._f3
            && (_f4 & other._f4) == other._f4
            && (_f5 & other._f5) == other._f5
            && (_f6 & other._f6) == other._f6
            && (_f7 & other._f7) == other._f7
            && (_f8 & other._f8) == other._f8;
    }

    public bool CheckAnyFrom(BitSet other)
    {
        return (_f1 & other._f1) != 0UL
            || (_f2 & other._f2) != 0UL
            || (_f3 & other._f3) != 0UL
            || (_f4 & other._f4) != 0UL
            || (_f5 & other._f5) != 0UL
            || (_f6 & other._f6) != 0UL
            || (_f7 & other._f7) != 0UL
            || (_f8 & other._f8) != 0UL;
    }

    public bool Equals(BitSet? other)
    {
        return _f1 == other!._f1
            && _f2 == other!._f2
            && _f3 == other!._f3
            && _f4 == other!._f4
            && _f5 == other!._f5
            && _f6 == other!._f6
            && _f7 == other!._f7
            && _f8 == other!._f8;
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (ReferenceEquals(null, other))
        {
            return false;
        }

        return other is BitSet otherOpt && Equals(otherOpt);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = (int) 2166136261;
            hash = (hash * 16777619) ^ _f1.GetHashCode();
            hash = (hash * 16777619) ^ _f2.GetHashCode();
            hash = (hash * 16777619) ^ _f3.GetHashCode();
            hash = (hash * 16777619) ^ _f4.GetHashCode();
            hash = (hash * 16777619) ^ _f5.GetHashCode();
            hash = (hash * 16777619) ^ _f6.GetHashCode();
            hash = (hash * 16777619) ^ _f7.GetHashCode();
            hash = (hash * 16777619) ^ _f8.GetHashCode();
            return hash;
        }
    }

    public static bool operator ==(BitSet a, BitSet b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(BitSet a, BitSet b)
    {
        return !a.Equals(b);
    }
}
