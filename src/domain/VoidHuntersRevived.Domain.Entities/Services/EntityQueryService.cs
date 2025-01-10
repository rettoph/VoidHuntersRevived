using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Serilog;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    public class EntityQueryService(ILogger logger) : IEntityQueryService, IQueryingEntitiesEngine
    {
        public EntitiesDB entitiesDB { get; set; } = null!;
        private readonly Dictionary<EntityGlobalId, EntityLocalId> _globalLocalIds = [];
        private readonly Dictionary<EntityLocalId, EntityGlobalId> _localGlobalIds = [];
        private readonly ILogger _logger = logger;

        public void Ready()
        {
            //
        }

        public EntityLocalId GetLocalId(EntityGlobalId globalId) => this._globalLocalIds[globalId];

        public bool TryGetLocalId(EntityGlobalId globalId, out EntityLocalId localId) => this._globalLocalIds.TryGetValue(globalId, out localId);

        public EntityGlobalId GetGlobalId(EntityLocalId localId) => this._localGlobalIds[localId];

        public bool TryGetGlobalId(EntityLocalId localId, out EntityGlobalId globalId) => this._localGlobalIds.TryGetValue(localId, out globalId);

        public bool TryGetEntity(EntityGlobalId globalId, out Entity entity)
        {
            if (this.TryGetLocalId(globalId, out EntityLocalId localId) == false)
            {
                entity = default;
                return false;
            }

            _ = this.QueryByEGID<EntityLocalId>(localId.Value, out GroupIndex groupIndex);

            entity = new Entity(groupIndex.Index, localId, globalId);
            return true;
        }
        public bool TryGetEntity<T>(EntityGlobalId globalId, out Entity<T> entity)
            where T : unmanaged, IEntityComponent
        {
            try
            {
                if (this.TryGetLocalId(globalId, out EntityLocalId localId) == false)
                {
                    entity = default;
                    return false;
                }

                ref T component = ref this.QueryByEGID<T>(localId.Value, out GroupIndex groupIndex);

                entity = new Entity<T>(groupIndex.Index, localId, globalId, ref component);
                return true;

            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Error getting entity. EntityGlobalId = {EntityGlobalId}, Type = {Type}", globalId, typeof(T));

                entity = default;
                return false;
            }
        }

        public bool TryGetEntity(EntityLocalId localId, out Entity entity)
        {
            EntityGlobalId globalId = this.QueryByEGID<EntityGlobalId>(localId.Value, out GroupIndex groupIndex);

            entity = new Entity(groupIndex.Index, localId, globalId);
            return true;
        }
        public bool TryGetEntity<T>(EntityLocalId localId, out Entity<T> entity)
            where T : unmanaged, IEntityComponent
        {
            try
            {
                EntityGlobalId globalId = this.QueryByEGID<EntityGlobalId>(localId.Value, out GroupIndex groupIndex);
                ref T component = ref this.QueryByGroupIndex<T>(groupIndex);

                entity = new Entity<T>(groupIndex.Index, localId, globalId, ref component);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Error getting entity. EntityLocalId = {EntityLocalId}, Type = {Type}", localId, typeof(T));

                entity = default;
                return false;
            }
        }

        public bool TryGetEntity(ExclusiveGroupStruct groupId, uint index, out Entity entity)
        {
            var (localIds, globalIds, _) = this.QueryEntities<EntityLocalId, EntityGlobalId>(groupId);

            entity = new Entity(index, localIds[index], globalIds[index]);
            return true;
        }
        public bool TryGetEntity<T>(ExclusiveGroupStruct groupId, uint index, out Entity<T> entity)
            where T : unmanaged, IEntityComponent
        {
            try
            {
                var (localIds, globalIds, components, _) = this.QueryEntities<EntityLocalId, EntityGlobalId, T>(groupId);
                entity = new Entity<T>(index, localIds[index], globalIds[index], ref components[index]);
                return true;
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, "Error getting entity. ExclusiveGroupStruct = {ExclusiveGroupStruct}, Index = {Index}, Type = {Type}", groupId, index, typeof(T));

                entity = default;
                return false;
            }
        }

        public ref EntityLocalId AddLocalId(EntityGlobalId globalId)
        {
            ref EntityLocalId localId = ref CollectionsMarshal.GetValueRefOrAddDefault(this._globalLocalIds, globalId, out bool exists);
            if (exists == true)
            { // Unable to hard spawn - entity already exists
                throw new NotImplementedException();
            }

            this._logger.Verbose("Added EntityGlobalId {EntityGlobalId}", globalId);
            return ref localId;
        }

        public void AddGlobalId(EntityLocalId localId, EntityGlobalId globalId) => this._localGlobalIds.Add(localId, globalId);

        public bool Remove(EntityGlobalId globalId)
        {
            if (this._globalLocalIds.Remove(globalId, out EntityLocalId localId)
                && this._localGlobalIds.Remove(localId))
            {
                this._logger.Verbose("Removed EntityGlobalId {EntityGlobalId}", globalId);
                return true;
            }

            throw new Exception();
        }

        public bool TryQueryByEGID<T>(EGID egid, out T value)
            where T : unmanaged, IEntityComponent => this.entitiesDB.TryGetEntity<T>(egid, out value);

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

        public bool Has<T>(ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent => this.entitiesDB.HasAny<T>(groupID);

        public bool Has<T1>(ExclusiveGroupStruct groupId, out EntityCollection<T1> entities)
            where T1 : unmanaged, IEntityComponent => this.entitiesDB.HasAny(groupId, out entities);

        public bool HasAll<T1, T2>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent => this.entitiesDB.HasAll(groupId, out entities);

        public bool HasAll<T1, T2, T3>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent => this.entitiesDB.HasAll(groupId, out entities);

        public bool HasAll<T1, T2, T3, T4>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3, T4> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent => this.entitiesDB.HasAll(groupId, out entities);

        public ref T QueryByGroupIndex<T>(GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
        {
            var (entities, _) = this.entitiesDB.QueryEntities<T>(groupIndex.GroupID);

            return ref entities[groupIndex.Index];
        }

        public bool TryQueryByGroupIndex<T>(GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (!this.entitiesDB.HasAny<T>(groupIndex.GroupID))
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

        public bool TryQueryByGroupIndex<T>(ExclusiveGroupStruct groupId, uint index, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (!this.entitiesDB.HasAny<T>(groupId))
            {
                value = default;
                return false;
            }

            value = this.QueryByGroupIndex<T>(groupId, index);
            return true;
        }

        public EntityCollection<T1> QueryEntities<T1>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1>(groupID);

        public EntityCollection<T1, T2> QueryEntities<T1, T2>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2>(groupID);

        public EntityCollection<T1, T2, T3> QueryEntities<T1, T2, T3>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3>(groupID);

        public EntityCollection<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(ExclusiveGroupStruct groupID)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3, T4>(groupID);

        public GroupsEnumerable<T1> QueryEntities<T1>()
            where T1 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1>(EntityGroupList<T1>.Values);

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2>(EntityGroupList<T1, T2>.Values);

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3>(EntityGroupList<T1, T2, T3>.Values);

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3, T4>(EntityGroupList<T1, T2, T3, T4>.Values);

        public GroupsEnumerable<T1> QueryEntities<T1>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1>(groups);

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2>(groups);

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3>(groups);

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent => this.entitiesDB.QueryEntities<T1, T2, T3, T4>(groups);

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1>()
            where T1 : unmanaged, IEntityComponent => EntityGroupList<T1>.Values;

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent => EntityGroupList<T1, T2>.Values;

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent => EntityGroupList<T1, T2, T3>.Values;

        public LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent => EntityGroupList<T1, T2, T3, T4>.Values;

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

        public bool IsSpawned(GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(groupIndex, out EntityStatus status))
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

        public bool IsDespawned(GroupIndex groupIndex)
        {
            if (this.TryQueryByGroupIndex<EntityStatus>(groupIndex, out EntityStatus status))
            {
                return status.IsDespawned;
            }

            return false;
        }

        public ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId);

            return ref filter;
        }
    }
}