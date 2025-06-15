namespace NEconomicon.Tests;

[TestFixture]
public class BitSetTests
{
    [Test]
    public void BitSet_FromEmptyToFull()
    {
        var bitSet = BitSet.Empty();
        for (var i = BitSet.MinIndex; i <= BitSet.MaxIndex; i++)
        {
            Assert.That(bitSet.Check(i), Is.False);
        }

        for (var i = BitSet.MinIndex; i <= BitSet.MaxIndex; i++)
        {
            bitSet.Set(i);
            for (var j = BitSet.MinIndex; j <= BitSet.MaxIndex; j++)
            {
                Assert.That(bitSet.Check(j), Is.EqualTo(j <= i));
            }
        }
    }

    [Test]
    public void BitSet_FromFullToEmpty()
    {
        var bitSet = BitSet.Full();
        for (var i = BitSet.MinIndex; i <= BitSet.MaxIndex; i++)
        {
            Assert.That(bitSet.Check(i), Is.True);
        }

        for (var i = BitSet.MinIndex; i <= BitSet.MaxIndex; i++)
        {
            bitSet.Clear(i);
            for (var j = BitSet.MinIndex; j <= BitSet.MaxIndex; j++)
            {
                // System.Console.WriteLine($"{i}|{j}: {bitSet.Check(j)}");
                Assert.That(bitSet.Check(j), Is.EqualTo(j > i));
            }
        }
    }
}

