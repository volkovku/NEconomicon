namespace NEconomicon.Tests.Storage;

using NEconomicon.Model;
using NEconomicon.Storage;

public class EntityDataStorageTests
{
    private EntityDataStorage CreateStorage()
        => new EntityDataStorage(16, 8);

    [Test]
    public async Task NewEntity_ShouldCreateEntity_WhenTransactionStarted()
    {
        var storage = CreateStorage();

        storage.StartTransaction();

        var entity = storage.NewEntity();

        storage.CommitTransaction();

        var found = storage.TryGetEntityById(entity.Id, out var loaded);

        await Assert.That(found).IsTrue();
        await Assert.That(entity.Id).IsEqualTo(loaded.Id);
    }

    [Test]
    public async Task NewEntity_ShouldRollback_WhenTransactionRolledBack()
    {
        var storage = CreateStorage();

        storage.StartTransaction();

        var entity = storage.NewEntity();

        storage.RollbackTransaction();

        var found = storage.TryGetEntityById(entity.Id, out _);

        await Assert.That(found).IsFalse();
    }

    [Test]
    public async Task GetEntityById_ShouldReturnEntity_WhenExists()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.CommitTransaction();

        var loaded = storage.GetEntityById(entity.Id);

        await Assert.That(entity.Id).IsEqualTo(loaded.Id);
    }

    [Test]
    public async Task RemoveEntity_ShouldMarkEntityRemoved()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        var removed = storage.RemoveEntity(entity.Id);
        storage.CommitTransaction();

        var found = storage.TryGetEntityById(entity.Id, out _);
        await Assert.That(removed).IsTrue();
        await Assert.That(found).IsFalse();
    }

    [Test]
    public async Task AddComponent_ShouldAddComponent()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        var added = storage.AddComponent(entity.Id, 5);
        storage.CommitTransaction();

        await Assert.That(added).IsTrue();
        await Assert.That(storage.HasComponent(entity.Id, 5)).IsTrue();
    }

    [Test]
    public async Task RemoveComponent_ShouldRemoveComponent()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.AddComponent(entity.Id, 5);
        var removed = storage.RemoveComponent(entity.Id, 5);
        storage.CommitTransaction();

        await Assert.That(removed).IsTrue();
        await Assert.That(storage.HasComponent(entity.Id, 5)).IsFalse();
    }

    [Test]
    public async Task SetPropertyValue_ShouldCreateProperty()
    {
        var storage = CreateStorage();
        storage.StartTransaction();
    
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 2, 100);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 2, out var value);

        await Assert.That(found).IsTrue();
        await Assert.That(value).IsEqualTo(100);
    }

    [Test]
    public async Task SetPropertyValue_ShouldOverwriteProperty()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 1, 10);
        storage.SetPropertyValue(entity.Id, 1, 1, 20);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 1, out var value);

        await Assert.That(found).IsTrue();
        await Assert.That(value).IsEqualTo(20);
    }

    [Test]
    public async Task RemoveComponent_ShouldRemoveProperties()
    {
        var storage = CreateStorage();
        storage.StartTransaction();

        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 2, 1, 42);
        storage.RemoveComponent(entity.Id, 2);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 2, 1, out _);
        await Assert.That(found).IsTrue();
    }

    [Test]
    public async Task TransactionRollback_ShouldRestorePropertyValue()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 1, 10);
        storage.CommitTransaction();

        storage.StartTransaction();
        storage.SetPropertyValue(entity.Id, 1, 1, 50);
        storage.RollbackTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 1, out var value);

        await Assert.That(found).IsTrue();
        await Assert.That(value).IsEqualTo(10);
    }

    [Test]
    public async Task TransactionCommit_ShouldPersistPropertyValue()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 1, 10);
        storage.CommitTransaction();

        storage.StartTransaction();
        storage.SetPropertyValue(entity.Id, 1, 1, 50);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 1, out var value);

        await Assert.That(found).IsTrue();
        await Assert.That(value).IsEqualTo(50);
    }

    [Test]
    public async Task TryGetEntity_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        var storage = CreateStorage();
        var found = storage.TryGetEntityById(new EntityId(999), out _);
        await Assert.That(found).IsFalse();
    }
}