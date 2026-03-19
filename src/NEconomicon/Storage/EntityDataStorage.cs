namespace NEconomicon.Storage;

using System.Collections.Generic;
using NEconomicon.Collections;
using NEconomicon.Exceptions;
using NEconomicon.Model;

/// <summary>
/// Represents storage which manages entities data.
/// </summary>
public class EntityDataStorage
{
    private const int None = 0;

    private const byte EntryFlagNone = 0;
    private const byte EntryFlagTx = 1;
    private const byte EntryFlagTombstone = 1 << 1;

    // Entity as entries layout
    // [00] None - reserved
    // [01] Entity [prev_ix=00, next_ix=02]
    // [02] - Component [prev_ix=01, next_ix=03]
    // [03] -- Property [prev_ix=02, next_ix=04]
    // [04] -- Property [prev_ix=03, next_ix=05]
    // [05] -- Property [prev_ix=04, next_ix=06]
    // [06] - Component [prev_ix=05, next_ix=07]
    // [07] -- Property [prev_ix=06, next_ix=08]
    // [08] -- Property [prev_ix=07, next_ix=09]
    // [09] - Component [prev_ix=08, next_ix=00]
    // [10] Entity [prev_ix=00, next_ix=11]
    // [11] - Component [prev_ix=10, next_ix=00]
    private readonly Dictionary<ulong, int> _entryIndexByPackedId;
    private readonly Aos<ulong> _entryPackedId;
    private readonly Aos<int> _entryPrevIdx;
    private readonly Aos<int> _entryNextIdx;
    private readonly Aos<byte> _entryFlags;
    private readonly Aos<long> _entryValue;
    private readonly Aos<long> _entryValueBackup;

    private uint _nextEntityId;

    private bool _txStarted;
    private int _txEntriesCountOnStart;
    private uint _txNextEntityIdOnStart;
    private List<int> _txAffectedIndexes;

    /// <summary>
    /// Initializes a new instance of storage.
    /// </summary>
    /// <param name="initialCapacity">An initial capacity of storage.</param>
    /// <param name="growSize">A grow size of storage.</param>
    public EntityDataStorage(int initialCapacity, int growSize)
    {
        _entryIndexByPackedId = [];
        _entryPackedId = new Aos<ulong>(initialCapacity, new IncGrowStrategy<ulong>(growSize));
        _entryPrevIdx = new Aos<int>(initialCapacity, new IncGrowStrategy<int>(growSize));
        _entryNextIdx = new Aos<int>(initialCapacity, new IncGrowStrategy<int>(growSize));
        _entryFlags = new Aos<byte>(initialCapacity, new IncGrowStrategy<byte>(growSize));
        _entryValue = new Aos<long>(initialCapacity, new IncGrowStrategy<long>(growSize));
        _entryValueBackup = new Aos<long>(initialCapacity, new IncGrowStrategy<long>(growSize));

        _nextEntityId = 1;

        // 0 idx reserved as none placeholder
        _entryPackedId.AddValue(0); 
        _entryPrevIdx.AddValue(0);
        _entryNextIdx.AddValue(0);
        _entryFlags.AddValue(0);
        _entryValue.AddValue(0);
        _entryValueBackup.AddValue(0);

        _txStarted = false;
        _txAffectedIndexes = [];
    }

    /// <summary>
    /// Starts transaction.
    /// </summary>
    public void StartTransaction()
    {
        EnsureTransactionNotStarted();

        _txStarted = true;
        _txEntriesCountOnStart = _entryPackedId.Count;
        _txNextEntityIdOnStart = _nextEntityId;
    }

    /// <summary>
    /// Commits current transaction.
    /// If transaction not started throws exception.
    /// </summary>
    public void CommitTransaction()
    {
        EnsureTransactionStarted();

        if (_txAffectedIndexes.Count == 0)
        {
            CloseTransaction();
            return;
        }

        _txAffectedIndexes.Sort();
        for (var i = _txAffectedIndexes.Count - 1; i >= 0; i--)
        {
            var idx = _txAffectedIndexes[i];
            ref var flags = ref _entryFlags.GetAt(idx);
            if (IsMarkedWithTombstone(ref flags))
            {
                RemoveEntryAt(idx);
                continue;
            }

            flags = (byte)(flags & ~EntryFlagTx);
        }

        CloseTransaction();
    }

    /// <summary>
    /// Rollbacks current transaction.
    /// If transaction not started throws exception.
    /// </summary>
    public void RollbackTransaction()
    {
        if (_txAffectedIndexes.Count == 0)
        {
            CloseTransaction();
            return;
        }

        _txAffectedIndexes.Sort();
        for (var i = _txAffectedIndexes.Count - 1; i >= 0; i--)
        {
            var idx = _txAffectedIndexes[i];
            if (idx >= _txEntriesCountOnStart)
            {
                RemoveEntryAt(idx);
                continue;
            }

            ref var flags = ref _entryFlags.GetAt(idx);
            flags = 0;

            ref var value = ref _entryValue.GetAt(idx);
            value = _entryValueBackup.GetAt(idx);
        }

        _nextEntityId = _txNextEntityIdOnStart;
        CloseTransaction();
    }

    /// <summary>
    /// Creates new entity and returns it identity.
    /// </summary>
    /// <returns>Returns created entity.</returns>
    public Entity NewEntity()
    {
        EnsureTransactionStarted();

        var entityId = _nextEntityId++;
        AddEntry(GetPackedEntityId(entityId), None, None);

        return new Entity(new EntityId(entityId), this);
    }

    /// <summary>
    /// Returns entity associated with specified identifier.
    /// </summary>
    /// <param name="id">An identifier of requested entity.</param>
    /// <returns>Returns found entity.</returns>
    public Entity GetEntityById(EntityId id)
    {
        EnsureEntityIsAlive(id);
        return new Entity(id, this);
    }

    /// <summary>
    /// Try to get entity associated with specified identifier.
    /// If entity found returns true; otherwise false.
    /// </summary>
    /// <param name="id">An idetifier of requested entity.</param>
    /// <param name="entity">Found entity.</param>
    /// <returns></returns>
    public bool TryGetEntityById(EntityId id, out Entity entity)
    {
        var packedId = GetPackedEntityId(id.Value);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var idx))
        {
            entity = default;
            return false;
        }

        ref var f = ref _entryFlags.GetAt(idx);
        if (IsMarkedWithTombstone(ref f))
        {
            entity = default;
            return false;
        }

        entity = new Entity(id, this);
        return true;
    }

    /// <summary>
    /// Removes entity with specified identifier.
    /// Returns true if entity was alive before remove; otherwise false.
    /// </summary>
    /// <param name="id">An entity identity.</param>
    /// <returns>Returns true if entity was alive before remove; otherwise false.</returns>
    public bool RemoveEntity(EntityId id)
    {
        EnsureTransactionStarted();

        var packedId = GetPackedEntityId(id.Value);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var idx))
        {
            // Entity not found => removed
            return false;
        }

        ref var f = ref _entryFlags.GetAt(idx);
        if (IsMarkedWithTombstone(ref f))
        {
            // Entity already removed in this transaction
            return false;
        }

        SetTombstone(ref f);
        SetChangedInTransaction(ref f, idx);
        return true;
    }

    /// <summary>
    /// Adds component to entity.
    /// Returns true if new component was added; otherwise false.
    /// </summary>
    /// <param name="entityId">An entity identity.</param>
    /// <param name="componentId">A component identity.</param>
    /// <returns>Returns true if new component was added; otherwise false.</returns>
    public bool AddComponent(EntityId entityId, ushort componentId)
    {
        EnsureTransactionStarted();
        return AddComponentInternal(entityId, componentId, out _);
    }

    /// <summary>
    /// Removes component from entity.
    /// Returns true if component was removed; otherwise false.
    /// </summary>
    /// <param name="entityId">An entity identity.</param>
    /// <param name="componentId">A component identity.</param>
    /// <returns>Returns true if component was removed; otherwise false.</returns>
    public bool RemoveComponent(EntityId entityId, ushort componentId)
    {
        EnsureTransactionStarted();
        EnsureEntityIsAlive(entityId);

        var packedId = GetPackedComponentId(entityId.Value, componentId);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var idx))
        {
            return false;
        }

        ref var f = ref _entryFlags.GetAt(idx);
        if (IsMarkedWithTombstone(ref f))
        {
            return false;
        }

        SetTombstone(ref f);
        SetChangedInTransaction(ref f, idx);
        RemoveComponentProperties(idx);

        return true;
    }

    /// <summary>
    /// Determines is entity with specified identifier has component.
    /// </summary>
    /// <param name="entityId">An entity identity.</param>
    /// <param name="componentId">A component identity.</param>
    /// <returns>Returns true if entity has component; othwerwise false.</returns>
    public bool HasComponent(EntityId entityId, ushort componentId)
    {
        var packedId = GetPackedComponentId(entityId.Value, componentId);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var idx))
        {
            return false;
        }

        ref var f = ref _entryFlags.GetAt(idx);
        return !IsMarkedWithTombstone(ref f);
    }

    /// <summary>
    /// Tryes to get entity component value.
    /// Returns true if values was found; otherwise false.
    /// </summary>
    /// <param name="entityId">An entity identity.</param>
    /// <param name="componentId">A component identity.</param>
    /// <param name="propertyId">A property identity.</param>
    /// <param name="value">Found value.</param>
    /// <returns>Returns true if values was found; otherwise false.</returns>
    public bool TryGetPropertyValue(EntityId entityId, ushort componentId, byte propertyId, out long value)
    {
        EnsureEntityIsAlive(entityId);

        var packedId = GetPackedPropertyId(entityId.Value, componentId, propertyId);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var idx))
        {
            value = default;
            return false;
        }

        ref var f = ref _entryFlags.GetAt(idx);
        if (IsMarkedWithTombstone(ref f))
        {
            value = default;
            return false;
        }

        value = _entryValue.GetAt(idx);
        return true;
    }

    /// <summary>
    /// Sets entity component property value.
    /// </summary>
    /// <param name="entityId">An entity identity.</param>
    /// <param name="componentId">A component identity.</param>
    /// <param name="propertyId">A property identity.</param>
    /// <param name="value">A value to set.</param>
    public void SetPropertyValue(EntityId entityId, ushort componentId, byte propertyId, long value)
    {
        EnsureTransactionStarted();

        AddComponentInternal(entityId, componentId, out var componentIdx);

        var packedId = GetPackedPropertyId(entityId.Value, componentId, propertyId);
        if (!_entryIndexByPackedId.TryGetValue(packedId, out var propertyIdx))
        {
            propertyIdx = AddEntry(packedId, componentIdx, _entryNextIdx.GetAt(componentIdx));
            _entryValue.SetAt(propertyIdx, value);
            return;
        }

        ref var f = ref _entryFlags.GetAt(propertyIdx);
        if (IsChangedInTransaction(ref f))
        {
            _entryValue.SetAt(propertyIdx, value);
            RemoveTombstone(ref f);
            return;
        }

        ref var propertyValue = ref _entryValue.GetAt(propertyIdx);
        _entryValueBackup.SetAt(propertyIdx, propertyValue);
        propertyValue = value;
        SetChangedInTransaction(ref f, propertyIdx);

        return;
    }

    private bool AddComponentInternal(EntityId entityId, ushort componentId, out int componentIdx)
    {
        var entityIdx = EnsureEntityIsAlive(entityId);
        var componentPackedId = GetPackedComponentId(entityId.Value, componentId);
        if (!_entryIndexByPackedId.TryGetValue(componentPackedId, out componentIdx))
        {
            componentIdx = AddEntry(componentPackedId, entityIdx, _entryNextIdx.GetAt(entityIdx));
            return true;
        }

        ref var f = ref _entryFlags.GetAt(componentIdx);
        if (IsMarkedWithTombstone(ref f))
        {
            RemoveTombstone(ref f);
            return true;
        }

        return false;
    }

    private void RemoveComponentProperties(int componentIdx)
    {
        var idx = _entryNextIdx.GetAt(componentIdx);
        while (idx != None)
        {
            var packedId = _entryPackedId.GetAt(idx);
            if (!IsPropertyPackedId(packedId))
            {
                return;
            }

            ref var f = ref _entryFlags.GetAt(idx);
            if (IsChangedInTransaction(ref f))
            {
                SetTombstone(ref f);
                idx = _entryNextIdx.GetAt(idx);
                continue;
            }

            _entryValueBackup.SetAt(idx, _entryValue.GetAt(idx));
            SetTombstone(ref f);
            SetChangedInTransaction(ref f, idx);

            idx = _entryNextIdx.GetAt(idx);
        }
    }

    private int AddEntry(ulong packedId, int prevIdx, int nextIdx)
    {
        var idx = _entryPackedId.AddValue(packedId);
        if (nextIdx != None)
        {
            _entryPrevIdx.SetAt(nextIdx, idx);
        }

        _entryPrevIdx.AddValue(prevIdx);
        _entryNextIdx.AddValue(nextIdx);
        _entryFlags.AddValue(EntryFlagTx);
        _entryValue.AddValue(0);
        _entryValueBackup.AddValue(0);
        _entryIndexByPackedId.Add(packedId, idx);
        _txAffectedIndexes.Add(idx);

        return idx;
    }

    private void RemoveEntryAt(int idx)
    {
        // Remove index
        var packedId = _entryPackedId.GetAt(idx);
        _entryIndexByPackedId.Remove(packedId);

        // Unlink
        var prev = _entryPrevIdx.GetAt(idx);
        var next = _entryNextIdx.GetAt(idx);
        if (prev != None)
        {
            _entryNextIdx.SetAt(prev, next);
        }

        if (next != None)
        {
            _entryPrevIdx.SetAt(next, prev);
        }

        var lastIdx = _entryPackedId.Count - 1;
        if (idx != lastIdx)
        {
            // Move last entry to this idx
            var movedPackedId = _entryPackedId.GetAt(lastIdx);
            var movedPrev = _entryPrevIdx.GetAt(lastIdx);
            var movedNext = _entryNextIdx.GetAt(lastIdx);
            var movedFlags = _entryFlags.GetAt(lastIdx);
            var movedValue = _entryValue.GetAt(lastIdx);
            var movedBackup = _entryValueBackup.GetAt(lastIdx);

            _entryPackedId.SetAt(idx, movedPackedId);
            _entryPrevIdx.SetAt(idx, movedPrev);
            _entryNextIdx.SetAt(idx, movedNext);
            _entryFlags.SetAt(idx, movedFlags);
            _entryValue.SetAt(idx, movedValue);
            _entryValueBackup.SetAt(idx, movedBackup);

            _entryIndexByPackedId[movedPackedId] = idx;

            if (movedPrev != None)
            {
                _entryNextIdx.SetAt(movedPrev, idx);
            }

            if (movedNext != None)
            {
                _entryPrevIdx.SetAt(movedNext, idx);
            }
        }

        _entryPackedId.RemoveLast();
        _entryPrevIdx.RemoveLast();
        _entryNextIdx.RemoveLast();
        _entryFlags.RemoveLast();
        _entryValue.RemoveLast();
        _entryValueBackup.RemoveLast();
    }

    // Packed entity id
    // 8bit  |      : Alligment
    // 32bit | << 24: EntityId
    // 16bit | << 8 : ComponentId
    // 8bit  | << 0 : PropertyId
    private static ulong GetPackedEntityId(uint entityId)
        => (ulong)entityId << 24;

    private static ulong GetPackedComponentId(uint entityId, ushort componentId)
        => ((ulong)entityId << 24) | ((ulong)componentId << 8);

    private static ulong GetPackedPropertyId(uint entityId, ushort componentId, byte propertyId)
        => ((ulong)entityId << 24) | ((ulong)componentId << 8) | propertyId;

    private static bool IsPropertyPackedId(ulong packedId) => (packedId & byte.MaxValue) != 0;

    private static bool IsMarkedWithTombstone(ref byte f) => IsMarkedWith(ref f, EntryFlagTombstone);
    private static void SetTombstone(ref byte f) => f |= EntryFlagTombstone;
    private static void RemoveTombstone(ref byte f) => f = (byte)(f & ~EntryFlagTombstone);

    private static bool IsChangedInTransaction(ref byte f) => IsMarkedWith(ref f, EntryFlagTx);

    private void SetChangedInTransaction(ref byte f, int idx)
    {
        if (IsChangedInTransaction(ref f))
        {
            return;
        }

        f |= EntryFlagTx;
        _txAffectedIndexes.Add(idx);
    }

    private static bool IsMarkedWith(ref byte flags, byte flag) => (flags & flag) == flag;

    private void CloseTransaction()
    {
        _txAffectedIndexes.Clear();
        _txStarted = false;
    }

    private void EnsureTransactionNotStarted()
    {
        if (_txStarted)
        {
            throw new InvalidOperationException("Transaction already started");
        }
    }

    private void EnsureTransactionStarted()
    {
        if (!_txStarted)
        {
            throw new InvalidOperationException("Transaction not started");
        }
    }

    private int EnsureEntityIsAlive(EntityId entityId)
    {
        if (!_entryIndexByPackedId.TryGetValue(GetPackedEntityId(entityId.Value), out var idx))
        {
            return Throw.EntityNotFound<int>(entityId);
        }

        ref var flags = ref _entryFlags.GetAt(idx);
        if ((flags & EntryFlagTombstone) == EntryFlagTombstone)
        {
            return Throw.EntityNotFound<int>(entityId);
        }

        return idx;
    }
}