using NEconomicon.Model;
using PropertyAttribute = NEconomicon.Model.PropertyAttribute;

namespace NEconomicon.Tests.Model;

public class EntityDataStorageComplexTests
{
    [Component(id: 1)]
    public class Progress : Component<Progress>
    {
        [Property(id: 1)] 
        public readonly Property<int> Level = null!;

        [Property(id: 2)] 
        public readonly Property<int> Exp = null!;
    }

    [Test]
    public void EntityDataStorageComplexTest()
    {
        var storage = new EntityDataStorage(100, 100);
        storage.StartTransaction();

        var entity = storage.NewEntity();
        entity.Set(Progress._.Level, 1);
        entity.Set(Progress._.Exp, 0);

        Assert.That(entity.Get(Progress._.Level), Is.EqualTo(1));
    }
}