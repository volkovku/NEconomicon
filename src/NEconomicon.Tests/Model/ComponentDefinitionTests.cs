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
    public async Task Component_ShouldDescribeItSelf()
    {
        var ci1 = TestComponent1._;
        await Assert.That(ci1.Property1.ComponentId).EqualTo<ushort>(1);
        await Assert.That(ci1.Property1.Id).EqualTo<byte>(1);
        await Assert.That(ci1.Property1.Name).EqualTo("Property1");
        await Assert.That(ci1.Property2.ComponentId).EqualTo<ushort>(1);
        await Assert.That(ci1.Property2.Id).EqualTo<byte>(2);
        await Assert.That(ci1.Property2.Name).EqualTo("Property2SpecifiedName");

        var cd1 = TestComponent1.D;
        await Assert.That(cd1.Id).EqualTo<ushort>(1);
        await Assert.That(cd1.Properties.Count).EqualTo(2);
        await Assert.That(cd1.Properties[0]).EqualTo(ci1.Property1);
        await Assert.That(cd1.Properties[1]).EqualTo(ci1.Property2);

        var ci2 = TestComponent2._;
        await Assert.That(ci2.Property1.ComponentId).EqualTo<ushort>(2);
        await Assert.That(ci2.Property1.Id).EqualTo<byte>(1);
        await Assert.That(ci2.Property1.Name).EqualTo("Property1SpecifiedName");
        await Assert.That(ci2.Property2.ComponentId).EqualTo<ushort>(2);
        await Assert.That(ci2.Property2.Id).EqualTo<byte>(2);
        await Assert.That(ci2.Property2.Name).EqualTo("Property2");

        var cd2 = TestComponent2.D;
        await Assert.That(cd2.Id).EqualTo<ushort>(2);
        await Assert.That(cd2.Properties.Count).EqualTo(2);
        await Assert.That(cd2.Properties[0]).EqualTo(ci2.Property1);
        await Assert.That(cd2.Properties[1]).EqualTo(ci2.Property2);
    }
}