namespace NEconomicon.Tests.Storage;

using NEconomicon.Model;
using NEconomicon.Storage;
using PropertyAttribute = Model.PropertyAttribute;

public class EntityDataStorageComplexTests
{
    [Component(id: 1)]
    public class Progress
    {
        [Property(id: 1)]
        public required Property<int> Level { get; set; }

        [Property(id: 2)]
        public required Property<int> Exp { get; set; }
    }

    [Test]
    public async Task EntityDataStorageComplexTest()
    {
        var storage = new EntityDataStorage(100, 100);
        storage.StartTransaction();

        var entity = storage.NewEntity();
        entity.Set<Progress>(_ => _.Level, 1);
        entity.Set<Progress>(_ => _.Exp, 0);
    }
}