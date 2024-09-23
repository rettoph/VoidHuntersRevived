using Guppy.Core.Common.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    public class EntityQueryService : IEntityQueryService, IQueryingEntitiesEngine
    {
        public EntitiesDB entitiesDB { get; set; } = null!;
        private readonly Dictionary<VhId, EntityId> _ids = new Dictionary<VhId, EntityId>();

        public void Ready()
        {
            //
        }

        public EntityId GetId(VhId vhid)
        {
            return _ids[vhid];
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {
            return _ids.TryGetValue(vhid, out id);
        }

        public ref EntityId GetOrAddId(VhId vhid, out bool exists)
        {
            return ref CollectionsMarshal.GetValueRefOrAddDefault(_ids, vhid, out exists);
        }

        public ref EntityId AddId(VhId vhid)
        {
            ref EntityId id = ref this.GetOrAddId(vhid, out bool exists);
            if (exists == true)
            { // Unable to hard spawn - entity already exists
                throw new NotImplementedException();
            }

            return ref id;
        }

        public bool AddId(EntityId id)
        {
            if (_ids.TryAdd(id.VhId, id))
            {
                return true;
            }

            throw new Exception();
        }

        public bool RemoveId(EntityId id)
        {
            if (_ids.Remove(id.VhId))
            {
                return true;
            }

            throw new Exception();
        }

        public bool TryQueryById<T>(EntityId id, out T value)
            where T : unmanaged, IEntityComponent
        {
            return this.entitiesDB.TryGetEntity<T>(id.EGID, out value);
        }

        public bool TryQueryById<T>(EntityId id, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (this.entitiesDB.TryQueryEntitiesAndIndex<T>(id.EGID, out uint index, out var components))
            {
                value = components[index];
                groupIndex = new GroupIndex(id.EGID.groupID, index);

                return true;
            }

            value = default;
            groupIndex = default;
            return false;
        }

        public ref T QueryById<T>(EntityId id)
            where T : unmanaged, IEntityComponent
        {
            var components = this.entitiesDB.QueryEntitiesAndIndex<T>(id.EGID, out uint index);

            return ref components[index];
        }

        public ref T QueryById<T>(EntityId id, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
        {
            var components = this.entitiesDB.QueryEntitiesAndIndex<T>(id.EGID, out uint index);

            groupIndex = new GroupIndex(id.EGID.groupID, index);

            return ref components[index];
        }

        public ref T QueryById<T>(EntityId id, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent
        {
            if (this.entitiesDB.TryQueryEntitiesAndIndex<T>(id.EGID, out uint index, out var components))
            {
                exists = true;
                groupIndex = new GroupIndex(id.EGID.groupID, index);
                return ref components[index];
            }

            groupIndex = default!;
            exists = false;
            return ref Unsafe.NullRef<T>(); ;
        }

        public bool HasAny<T>(ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAny<T>(groupID);
        }

        public bool HasAny<T1>(ExclusiveGroupStruct groupId, out EntityCollection<T1> entities)
            where T1 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAny(groupId, out entities);
        }

        public bool HasAll<T1, T2>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAll(groupId, out entities);
        }

        public bool HasAll<T1, T2, T3>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAll(groupId, out entities);
        }

        public bool HasAll<T1, T2, T3, T4>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3, T4> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAll(groupId, out entities);
        }

        public ref T QueryByGroupIndex<T>(in GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
        {
            var (entities, _) = this.entitiesDB.QueryEntities<T>(groupIndex.GroupID);

            return ref entities[groupIndex.Index];
        }

        public bool TryQueryByGroupIndex<T>(in GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (!entitiesDB.HasAny<T>(groupIndex.GroupID))
            {
                value = default;
                return false;
            }

            value = this.QueryByGroupIndex<T>(groupIndex);
            return true;
        }

        public ref T QueryByGroupIndex<T>(ExclusiveGroupStruct groupId, uint index)
            where T : unmanaged, IEntityComponent
        {
            var (entities, _) = this.entitiesDB.QueryEntities<T>(groupId);

            return ref entities[index];
        }

        public bool TryQueryByGroupIndex<T>(in ExclusiveGroupStruct groupId, uint index, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (!entitiesDB.HasAny<T>(groupId))
            {
                value = default;
                return false;
            }

            value = this.QueryByGroupIndex<T>(groupId, index);
            return true;
        }

        public EntityCollection<T1> QueryEntities<T1>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1>(groupID);
        }

        public EntityCollection<T1, T2> QueryEntities<T1, T2>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2>(groupID);
        }

        public EntityCollection<T1, T2, T3> QueryEntities<T1, T2, T3>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3>(groupID);
        }

        public EntityCollection<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3, T4>(groupID);
        }

        public GroupsEnumerable<T1> QueryEntities<T1>()
            where T1 : unmanaged, IEntityComponent
        {
            var groups = this.entitiesDB.FindGroups<T1>();
            return this.entitiesDB.QueryEntities<T1>(groups);
        }

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            var groups = this.entitiesDB.FindGroups<T1, T2>();
            return this.entitiesDB.QueryEntities<T1, T2>(groups);
        }

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            var groups = this.entitiesDB.FindGroups<T1, T2, T3>();
            return this.entitiesDB.QueryEntities<T1, T2, T3>(groups);
        }

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            var groups = this.entitiesDB.FindGroups<T1, T2, T3, T4>();
            return this.entitiesDB.QueryEntities<T1, T2, T3, T4>(groups);
        }

        public GroupsEnumerable<T1> QueryEntities<T1>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1>(groups);
        }

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2>(groups);
        }

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3>(groups);
        }

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3, T4>(groups);
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1>()
            where T1 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.FindGroups<T1>();
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.FindGroups<T1, T2>();
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.FindGroups<T1, T2, T3>();
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.FindGroups<T1, T2, T3, T4>();
        }

        public int CalculateTotal<T>()
            where T : unmanaged, IEntityComponent
        {
            var groups = this.FindGroups<T>();
            int total = 0;

            foreach (var ((_, count), _) in this.QueryEntities<T>(groups))
            {
                total += count;
            }

            return total;
        }

        public bool IsSpawned(EntityId id)
        {
            if (this.TryQueryById<EntityStatus>(id, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsSpawned(EntityId id, out GroupIndex groupIndex)
        {
            if (this.TryQueryById<EntityStatus>(id, out groupIndex, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsSpawned(in GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(in groupIndex, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsDespawned(EntityId id)
        {
            if (this.TryQueryById<EntityStatus>(id, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public bool IsDespawned(EntityId id, out GroupIndex groupIndex)
        {
            if (this.TryQueryById<EntityStatus>(id, out groupIndex, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public bool IsDespawned(in GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(in groupIndex, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(unchecked((int)id.EGID.entityID), filterContext);

            return ref filter;
        }

        public ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId);

            return ref filter;
        }
    }
}
