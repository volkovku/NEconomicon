namespace NEconomicon.Tests.Storage;

using NEconomicon.Model;
using NEconomicon.Storage;
using PropertyAttribute = NEconomicon.Model.PropertyAttribute;

public class EntityDataStorageComplexTests
{
    [Component(id: 1)]
    public class Progress : Component<Progress>
    {
        [Property(id: 1)]
        public Property<int> Level { get; set; }

        [Property(id: 2)]
        public Property<int> Exp { get; set; }
    }

    [Test]
    public async Task EntityDataStorageComplexTest()
    {
        var storage = new EntityDataStorage(100, 100);
        storage.StartTransaction();

        var entity = storage.NewEntity();
        entity.Set(Progress._.Level, 1);
        entity.Set(Progress._.Exp, 0);

        await Assert.That(entity.Get(Progress._.Level)).IsEqualTo(1);
    }
}