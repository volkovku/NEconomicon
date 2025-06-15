namespace NEconomicon.Tests.Storage;

using NEconomicon.Storage;

[TestFixture]
public class ComponentInfoTests
{
    [Test]
    public void ComponentInfo_ShouldCreateComponentInfo()
    {
        CheckCompInfo(typeof(Component1), 1);
        CheckCompInfo(typeof(Component2), 2);
    }

    private static void CheckCompInfo(Type compType, ulong requiredKey)
    {
        var compInfo = ComponentInfo.Create(0, compType);
        Assert.That(compInfo.Key, Is.EqualTo(new ComponentKey(requiredKey)));
        Assert.That(compInfo.Type, Is.EqualTo(compType));
        Assert.That(compInfo.Name, Is.EqualTo(compType.Name));

        var inst1 = compInfo.New();
        Assert.That(inst1.GetType(), Is.EqualTo(compType));

        var inst2 = compInfo.New();
        Assert.That(inst2.GetType(), Is.EqualTo(compType));
        Assert.That(inst2.Equals(inst1), Is.False);
    }

    [ComponentKey(1)]
    private class Component1 : IComponent<Component1>
    {
    }

    [ComponentKey(2)]
    private class Component2 : IComponent<Component2>
    {
    }
}
