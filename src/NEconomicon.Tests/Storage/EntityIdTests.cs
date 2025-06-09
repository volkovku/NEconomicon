namespace NEconomicon.Tests.Storage;

using NEconomicon.Storage;

[TestFixture]
public class EntityIdTests
{
    [Test]
    public void EntityId_CommonTests()
    {
        var sIdA1 = new EntityId("A");
        var sIdA2 = new EntityId("A");
        var sIdB = new EntityId("B");
        var nId1 = new EntityId(1);
        var nId2 = new EntityId(2);
        var nId22 = nId1.Next();
        
        Assert.That(sIdA1.IsSymbolic, Is.True);
        Assert.That(sIdA1.IsNumeric, Is.False);
        Assert.That(sIdA1.ToString(), Is.EqualTo("EntityId('a')"));
        Assert.That(sIdA1, Is.EqualTo(sIdA2));
        Assert.That(sIdA2, Is.EqualTo(sIdA1));
        Assert.That(sIdA1, Is.Not.EqualTo(sIdB));
        Assert.That(sIdA1, Is.Not.EqualTo(nId1));
        Assert.That(sIdA1, Is.Not.EqualTo(nId2));

        Assert.That(sIdA2.IsSymbolic, Is.True);
        Assert.That(sIdA2.IsNumeric, Is.False);
        Assert.That(sIdA2.ToString(), Is.EqualTo("EntityId('a')"));

        Assert.That(sIdB.IsSymbolic, Is.True);
        Assert.That(sIdB.IsNumeric, Is.False);
        Assert.That(sIdB.ToString(), Is.EqualTo("EntityId('b')"));

        Assert.That(nId1.IsSymbolic, Is.False);
        Assert.That(nId1.IsNumeric, Is.True);
        Assert.That(nId1.ToString(), Is.EqualTo("EntityId(1)"));

        Assert.That(nId2.IsSymbolic, Is.False);
        Assert.That(nId2.IsNumeric, Is.True);
        Assert.That(nId2.ToString(), Is.EqualTo("EntityId(2)"));
        Assert.That(nId2, Is.EqualTo(nId22));
        Assert.That(nId22, Is.EqualTo(nId2));

        Assert.That(nId22.IsSymbolic, Is.False);
        Assert.That(nId22.IsNumeric, Is.True);
        Assert.That(nId22.ToString(), Is.EqualTo("EntityId(2)"));
    }
}
