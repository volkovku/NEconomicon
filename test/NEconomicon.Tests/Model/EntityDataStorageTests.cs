using NEconomicon.Model;

namespace NEconomicon.Tests.Model;

public class EntityDataStorageTests
{
    private EntityDataStorage CreateStorage() => new(16, 8);

    [Test]
    public void NewEntity_ShouldCreateEntity_WhenTransactionStarted()
    {
        var storage = CreateStorage();

        storage.StartTransaction();

        var entity = storage.NewEntity();

        storage.CommitTransaction();

        var found = storage.TryGetEntityById(entity.Id, out var loaded);

        Assert.That(found, Is.True);
        Assert.That(entity.Id, Is.EqualTo(loaded.Id));
    }

    [Test]
    public void NewEntity_ShouldRollback_WhenTransactionRolledBack()
    {
        var storage = CreateStorage();

        storage.StartTransaction();

        var entity = storage.NewEntity();

        storage.RollbackTransaction();

        var found = storage.TryGetEntityById(entity.Id, out _);

        Assert.That(found, Is.False);
    }

    [Test]
    public void GetEntityById_ShouldReturnEntity_WhenExists()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.CommitTransaction();

        var loaded = storage.GetEntityById(entity.Id);

        Assert.That(entity.Id, Is.EqualTo(loaded.Id));
    }

    [Test]
    public void RemoveEntity_ShouldMarkEntityRemoved()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        var removed = storage.RemoveEntity(entity.Id);
        storage.CommitTransaction();

        var found = storage.TryGetEntityById(entity.Id, out _);
        Assert.That(removed, Is.True);
        Assert.That(found, Is.False);
    }

    [Test]
    public void AddComponent_ShouldAddComponent()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        var added = storage.AddComponent(entity.Id, 5);
        storage.CommitTransaction();

        Assert.That(added, Is.True);
        Assert.That(storage.HasComponent(entity.Id, 5), Is.True);
    }

    [Test]
    public void RemoveComponent_ShouldRemoveComponent()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.AddComponent(entity.Id, 5);
        var removed = storage.RemoveComponent(entity.Id, 5);
        storage.CommitTransaction();

        Assert.That(removed, Is.True);
        Assert.That(storage.HasComponent(entity.Id, 5), Is.False);
    }

    [Test]
    public void SetPropertyValue_ShouldCreateProperty()
    {
        var storage = CreateStorage();
        storage.StartTransaction();
    
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 2, 100);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 2, out var value);

        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo(100));
    }

    [Test]
    public void SetPropertyValue_ShouldOverwriteProperty()
    {
        var storage = CreateStorage();

        storage.StartTransaction();
        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 1, 1, 10);
        storage.SetPropertyValue(entity.Id, 1, 1, 20);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 1, 1, out var value);

        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo(20));
    }

    [Test]
    public void RemoveComponent_ShouldRemoveProperties()
    {
        var storage = CreateStorage();
        storage.StartTransaction();

        var entity = storage.NewEntity();
        storage.SetPropertyValue(entity.Id, 2, 1, 42);
        storage.RemoveComponent(entity.Id, 2);
        storage.CommitTransaction();

        var found = storage.TryGetPropertyValue(entity.Id, 2, 1, out _);
        Assert.That(found, Is.True);
    }

    [Test]
    public void TransactionRollback_ShouldRestorePropertyValue()
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

        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo(10));
    }

    [Test]
    public void TransactionCommit_ShouldPersistPropertyValue()
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

        Assert.That(found, Is.True);
        Assert.That(value, Is.EqualTo(50));
    }

    [Test]
    public void TryGetEntity_ShouldReturnFalse_WhenEntityDoesNotExist()
    {
        var storage = CreateStorage();
        var found = storage.TryGetEntityById(new EntityId(999), out _);
        Assert.That(found, Is.False);
    }
}