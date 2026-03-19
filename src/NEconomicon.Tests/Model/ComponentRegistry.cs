namespace NEconomicon.Tests.Model;

using NEconomicon.Model;
using PropertyAttribute = NEconomicon.Model.PropertyAttribute;

public class ComponentRegistryTests
{
    [Component(id: 1)]
    public sealed class TestComponent1 : Component<TestComponent1>
    {
        [Property(id: 1)]
        public Property<int> Property1 { get; set; }

        [Property(id: 2, name: "Property2SpecifiedName")]
        public Property<string> Property2 { get; set; }
    }

    [Test]
    public async Task ComponentRegistry_ShouldRegisterComponent()
    {
    }
}