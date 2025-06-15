namespace NEconomicon.Tests.Storage;

using NEconomicon.Storage;

[TestFixture]
public class StorageTests
{
    [Test]
    public void Storage_ShouldCreateEntityWithSymbolicId()
    {
        var storage = CreateStorage();
        Assert.That(storage.IsEmpty);

        var entity = storage.CreateEntity("monk");
        Assert.That(entity.Id, Is.EqualTo(new EntityId("monk")));
        Assert.That(entity.IsAlive);
        Assert.That(storage.EntitiesCount, Is.EqualTo(1));

        var someFoundEntity = storage.FindEntity(entity.Id);
        Assert.That(someFoundEntity.TryGet(out var foundEntity));
        Assert.That(foundEntity.Id, Is.EqualTo(entity.Id));

        var sameEntity = storage.GetOrCreateEntity("monk", out var isNew);
        Assert.That(sameEntity.Id, Is.EqualTo(entity.Id));
        Assert.That(sameEntity.IsAlive);
        Assert.That(storage.EntitiesCount, Is.EqualTo(1));
        Assert.That(!isNew);
    }

    [Test]
    public void Storage_ShouldCreateEntityWithNumericId()
    {
        var storage = CreateStorage();

        var entity1 = storage.CreateEntity();
        entity1.Get<Character>().BlueprintId = "monk";
        entity1.Get<Level>().Value = 1;
        entity1.Get<Exp>().Value = 0;

        Assert.That(storage.EntitiesCount, Is.EqualTo(1));

        var entity2 = storage.CreateEntity();
        entity2.Get<Character>().BlueprintId = "mage";
        entity2.Get<Level>().Value = 2;
        entity2.Get<Exp>().Value = 100;

        Assert.That(storage.EntitiesCount, Is.EqualTo(2));

        Assert.That(entity1.Id, Is.EqualTo(new EntityId(0)));
        Assert.That(entity1.IsAlive);
        Assert.That(entity1.Get<Character>().BlueprintId, Is.EqualTo("monk"));
        Assert.That(entity1.Get<Level>().Value, Is.EqualTo(1));
        Assert.That(entity1.Get<Exp>().Value, Is.EqualTo(0));

        Assert.That(entity2.Id, Is.EqualTo(new EntityId(1)));
        Assert.That(entity2.IsAlive);
        Assert.That(entity2.Get<Character>().BlueprintId, Is.EqualTo("mage"));
        Assert.That(entity2.Get<Level>().Value, Is.EqualTo(2));
        Assert.That(entity2.Get<Exp>().Value, Is.EqualTo(100));
    }

    [Test]
    public void Storage_ShouldReuseComponentsFromDestroyedEntities()
    {
        var storage = CreateStorage();

        var entity1 = storage.CreateEntity();
        var characterComp = entity1.Get<Character>();
        characterComp.BlueprintId = "monk";

        var levelComp = entity1.Get<Level>();
        levelComp.Value = 1;

        var expComp = entity1.Get<Exp>();
        expComp.Value = 0;

        storage.DestroyEntity(entity1);
        Assert.That(storage.IsEmpty);

        var entity2 = storage.CreateEntity();
        Assert.That(entity2.ComponentsCount, Is.EqualTo(0));
        Assert.That(ReferenceEquals(entity2.Get<Character>(), characterComp));
        Assert.That(ReferenceEquals(entity2.Get<Level>(), levelComp));
        Assert.That(ReferenceEquals(entity2.Get<Exp>(), expComp));

        // Components values should be reset to initial
        Assert.That(entity2.Get<Character>().BlueprintId, Is.EqualTo(""));
        Assert.That(entity2.Get<Level>().Value, Is.EqualTo(0));
        Assert.That(entity2.Get<Exp>().Value, Is.EqualTo(0));
    }

    [Test]
    public void Storage_Query_ShouldIndexAlreadyExistEntities()
    {
        var storage = CreateStorage();

        var entity0 = storage.CreateEntity();
        entity0.Get<Character>();
        entity0.Get<Level>();

        var entity1 = storage.CreateEntity();
        entity1.Get<Character>();
        entity1.Get<Level>();
        entity1.Get<Exp>();

        var entity2 = storage.CreateEntity();
        entity2.Get<Character>();
        entity2.Get<Exp>();

        var entity3 = storage.CreateEntity();
        entity3.Get<Level>();
        entity3.Get<Exp>();
        Assert.That(entity3.Has<Character>(), Is.False);

        var charactersQuery = storage.CreateQuery(q => q.Include<Character>());
        Assert.That(charactersQuery.Count, Is.EqualTo(3));

        var characters = charactersQuery.GetEntities();
        Assert.That(characters.Count, Is.EqualTo(3));
        Assert.That(characters.Any(e => e.Id == entity0.Id));
        Assert.That(characters.Any(e => e.Id == entity1.Id));
        Assert.That(characters.Any(e => e.Id == entity2.Id));
        Assert.That(characters.Any(e => e.Id == entity3.Id), Is.False);

        var charactersWithLevel = storage
            .CreateQuery(q => q.Include<Character>().Include<Level>())
            .GetEntities();

        Assert.That(charactersWithLevel.Count, Is.EqualTo(2));
        Assert.That(charactersWithLevel.Any(e => e.Id == entity0.Id));
        Assert.That(charactersWithLevel.Any(e => e.Id == entity1.Id));
        Assert.That(charactersWithLevel.Any(e => e.Id == entity2.Id), Is.False);
        Assert.That(charactersWithLevel.Any(e => e.Id == entity3.Id), Is.False);

        var charactersWithLevelWithoutExp = storage
            .CreateQuery(q => q
                .Include<Character>()
                .Include<Level>()
                .Exclude<Exp>())
            .GetEntities();

        Assert.That(charactersWithLevelWithoutExp.Count, Is.EqualTo(1));
        Assert.That(charactersWithLevelWithoutExp.Any(e => e.Id == entity0.Id));
        Assert.That(charactersWithLevelWithoutExp.Any(e => e.Id == entity1.Id), Is.False);
        Assert.That(charactersWithLevelWithoutExp.Any(e => e.Id == entity2.Id), Is.False);
        Assert.That(charactersWithLevelWithoutExp.Any(e => e.Id == entity3.Id), Is.False);
    }

    [Test]
    public void Storage_Query_ShouldReturnsSameQueriesForSimilarDefinitions()
    {
        var storage = CreateStorage();

        var charactersQuery1 = storage.CreateQuery(q => q.Include<Character>());
        var charactersQuery2 = storage.CreateQuery(q => q.Include<Character>());
        Assert.That(ReferenceEquals(charactersQuery1, charactersQuery2));

        var charactersWithLevelWithoutExp1 = storage
            .CreateQuery(q => q
                .Include<Character>()
                .Include<Level>()
                .Exclude<Exp>());

        var charactersWithLevelWithoutExp2 = storage
            .CreateQuery(q => q
                .Include<Character>()
                .Include<Level>()
                .Exclude<Exp>());

        Assert.That(ReferenceEquals(
            charactersWithLevelWithoutExp1,
            charactersWithLevelWithoutExp2));
    }

    [Test]
    public void Storage_Query_ShouldReflectEntityChanges()
    {
        var storage = CreateStorage();

        var query = storage
            .CreateQuery(q => q
                .Include<Character>()
                .Include<Level>()
                .Exclude<Exp>());

        Assert.That(query.Count, Is.EqualTo(0));

        var entity = storage.CreateEntity();
        Assert.That(query.Count, Is.EqualTo(0));

        entity.Get<Character>();
        Assert.That(query.Count, Is.EqualTo(0));

        entity.Get<Level>();
        Assert.That(query.Count, Is.EqualTo(1));

        entity.Get<Exp>();
        Assert.That(query.Count, Is.EqualTo(0));

        entity.Remove<Exp>();
        Assert.That(query.Count, Is.EqualTo(1));

        entity.Remove<Level>();
        Assert.That(query.Count, Is.EqualTo(0));

        entity.Get<Level>();
        Assert.That(query.Count, Is.EqualTo(1));

        storage.DestroyEntity(entity);
        Assert.That(entity.IsAlive, Is.False);
        Assert.That(query.Count, Is.EqualTo(0));
    }

    private static Storage CreateStorage()
    {
        var scheme = new Scheme();
        scheme.RegisterComponent<Character>();
        scheme.RegisterComponent<Level>();
        scheme.RegisterComponent<Exp>();

        var storage = new Storage(scheme);

        return storage;
    }

    [ComponentKey(1)]
    private class Character : IComponent<Character>
    {
        public string BlueprintId = "";
    }

    [ComponentKey(2)]
    private class Level : IComponent<Level>
    {
        public int Value = 0;
    }

    [ComponentKey(3)]
    private class Exp : IComponent<Exp>
    {
        public int Value = 0;
    }
}
