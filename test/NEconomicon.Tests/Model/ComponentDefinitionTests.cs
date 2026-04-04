namespace NEconomicon.Tests.Model;

using NEconomicon.Model;
using PropertyAttribute = NEconomicon.Model.PropertyAttribute;

public class ComponentDescriptionTests
{
    [Component(id: 1)]
    public sealed class TestComponent1 : Component<TestComponent1>
    {
        [Property(id: 1)]
        public readonly Property<int> Property1 = null!;

        [Property(id: 2, name: "Property2SpecifiedName")]
        public readonly Property<string> Property2 = null!;
    }

    [Component(id: 2)]
    public sealed class TestComponent2 : Component<TestComponent2>
    {
        [Property(id: 1, name: "Property1SpecifiedName")]
        public readonly Property<string> Property1 = null!;

        [Property(id: 2)]
        public readonly Property<long> Property2 = null!;
    }

    [Test]
    public void Component_ShouldDescribeItSelf()
    {
        var ci1 = TestComponent1._;
        Assert.That(ci1.Property1.ComponentId, Is.EqualTo(1));
        Assert.That(ci1.Property1.Id, Is.EqualTo(1));
        Assert.That(ci1.Property1.Name, Is.EqualTo("Property1"));
        Assert.That(ci1.Property2.ComponentId, Is.EqualTo(1));
        Assert.That(ci1.Property2.Id, Is.EqualTo(2));
        Assert.That(ci1.Property2.Name, Is.EqualTo("Property2SpecifiedName"));

        var cd1 = TestComponent1.D;
        Assert.That(cd1.Id, Is.EqualTo(1));
        Assert.That(cd1.Properties.Count, Is.EqualTo(2));
        Assert.That(cd1.Properties[0], Is.EqualTo(ci1.Property1));
        Assert.That(cd1.Properties[1], Is.EqualTo(ci1.Property2));

        var ci2 = TestComponent2._;
        Assert.That(ci2.Property1.ComponentId, Is.EqualTo(2));
        Assert.That(ci2.Property1.Id, Is.EqualTo(1));
        Assert.That(ci2.Property1.Name, Is.EqualTo("Property1SpecifiedName"));
        Assert.That(ci2.Property2.ComponentId, Is.EqualTo(2));
        Assert.That(ci2.Property2.Id, Is.EqualTo(2));
        Assert.That(ci2.Property2.Name, Is.EqualTo("Property2"));

        var cd2 = TestComponent2.D;
        Assert.That(cd2.Id, Is.EqualTo(2));
        Assert.That(cd2.Properties.Count, Is.EqualTo(2));
        Assert.That(cd2.Properties[0], Is.EqualTo(ci2.Property1));
        Assert.That(cd2.Properties[1], Is.EqualTo(ci2.Property2));
    }
}
