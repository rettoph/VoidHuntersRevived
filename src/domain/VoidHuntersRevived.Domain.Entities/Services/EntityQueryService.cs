using Svelto.DataStructures;
using Svelto.ECS;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityQueryService : IEntityQueryService, IQueryingEntitiesEngine
    {
        public EntitiesDB entitiesDB { get; set; } = null!;
        private readonly Dictionary<EntityGlobalId, EntityLocalId> _ids = [];

        public void Ready()
        {
            //
        }

        public EntityLocalId GetLocalId(EntityGlobalId globalId)
        {
            return _ids[globalId];
        }

        public bool TryGetLocalId(EntityGlobalId globalId, out EntityLocalId localId)
        {
            return _ids.TryGetValue(globalId, out localId);
        }

        public EntityId GetId(VhId vhid)
        {
            return new EntityId(_ids[vhid.ToGlobalEntityId()].Value, vhid);
        }

        public bool TryGetId(VhId vhid, out EntityId id)
        {

            if (_ids.TryGetValue(vhid.ToGlobalEntityId(), out EntityLocalId localId) == false)
            {
                id = default;
                return false;
            }

            id = new EntityId(_ids[vhid.ToGlobalEntityId()].Value, vhid);
            return true;
        }

        public ref EntityLocalId AddLocalId(EntityGlobalId globalId)
        {
            ref EntityLocalId localId = ref CollectionsMarshal.GetValueRefOrAddDefault(_ids, globalId, out bool exists);
            if (exists == true)
            { // Unable to hard spawn - entity already exists
                throw new NotImplementedException();
            }

            return ref localId;
        }

        public bool RemoveLocalId(EntityGlobalId globalId)
        {
            if (_ids.Remove(globalId))
            {
                return true;
            }

            throw new Exception();
        }

        public bool TryQueryByEGID<T>(EGID egid, out T value)
            where T : unmanaged, IEntityComponent
        {
            return this.entitiesDB.TryGetEntity<T>(egid, out value);
        }

        public bool TryQueryByEGID<T>(EGID egid, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (this.entitiesDB.TryQueryEntitiesAndIndex<T>(egid, out uint index, out var components))
            {
                value = components[index];
                groupIndex = new GroupIndex(egid.groupID, index);

                return true;
            }

            value = default;
            groupIndex = default;
            return false;
        }

        public ref T QueryByEGID<T>(EGID egid)
            where T : unmanaged, IEntityComponent
        {
            var components = this.entitiesDB.QueryEntitiesAndIndex<T>(egid, out uint index);

            return ref components[index];
        }

        public ref T QueryByEGID<T>(EGID egid, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
        {
            var components = this.entitiesDB.QueryEntitiesAndIndex<T>(egid, out uint index);

            groupIndex = new GroupIndex(egid.groupID, index);

            return ref components[index];
        }

        public ref T QueryByEGID<T>(EGID egid, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent
        {
            if (this.entitiesDB.TryQueryEntitiesAndIndex<T>(egid, out uint index, out var components))
            {
                exists = true;
                groupIndex = new GroupIndex(egid.groupID, index);
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
            return this.entitiesDB.QueryEntities<T1>(EntityGroupList<T1>.Values);
        }

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2>(EntityGroupList<T1, T2>.Values);
        }

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3>(EntityGroupList<T1, T2, T3>.Values);
        }

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return this.entitiesDB.QueryEntities<T1, T2, T3, T4>(EntityGroupList<T1, T2, T3, T4>.Values);
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
            return EntityGroupList<T1>.Values;
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
        {
            return EntityGroupList<T1, T2>.Values;
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
        {
            return EntityGroupList<T1, T2, T3>.Values;
        }

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent
        {
            return EntityGroupList<T1, T2, T3, T4>.Values;
        }

        public int CalculateTotal<T>()
            where T : unmanaged, IEntityComponent
        {
            int total = 0;

            foreach (var ((_, count), _) in this.QueryEntities<T>(EntityGroupList<T>.Values))
            {
                total += count;
            }

            return total;
        }

        public bool IsSpawned(EGID egid)
        {
            if (this.TryQueryByEGID<EntityStatus>(egid, out EntityStatus status))
            {
                return status.IsSpawned;
            }

            return false;
        }

        public bool IsSpawned(EGID egid, out GroupIndex groupIndex)
        {
            if (this.TryQueryByEGID<EntityStatus>(egid, out groupIndex, out EntityStatus status))
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

        public bool IsDespawned(EGID egid)
        {
            if (this.TryQueryByEGID<EntityStatus>(egid, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public bool IsDespawned(EGID egid, out GroupIndex groupIndex)
        {
            if (this.TryQueryByEGID<EntityStatus>(egid, out groupIndex, out EntityStatus status))
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

        public ref EntityFilterCollection GetFilter<T>(EGID egid, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(unchecked((int)egid.entityID), filterContext);

            return ref filter;
        }

        public ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId);

            return ref filter;
        }

        public ref EntityFilterCollection GetFilter<T>(EntityFilterId<T> filterId)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId.CombinedFilterId);

            return ref filter;
        }
    }
}
