namespace NEconomicon.Tests.Model;

using NEconomicon.Model;
using PropertyAttribute = NEconomicon.Model.PropertyAttribute;

public class ComponentRegistryTests
{
    [Component(id: 1)]
    public sealed class TestComponent1 : Component<TestComponent1>
    {
        [Property(id: 1)]
        public readonly Property<int> Property1 = null!;

        [Property(id: 2, name: "Property2SpecifiedName")]
        public readonly Property<string> Property2 = null!;
    }

    [Test]
    public async Task ComponentRegistry_ShouldRegisterComponent()
    {
    }
}