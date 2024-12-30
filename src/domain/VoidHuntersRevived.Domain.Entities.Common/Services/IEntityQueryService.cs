using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Utilities;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityQueryService
    {
        bool TryQueryByEGID<T>(EGID egid, out T value)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByEGID<T>(EGID egid, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent;

        ref T QueryByEGID<T>(EGID egid)
            where T : unmanaged, IEntityComponent;

        ref T QueryByEGID<T>(EGID egid, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent;

        ref T QueryByEGID<T>(EGID egid, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByLocalId<T>(EntityLocalId localId, out T value)
            where T : unmanaged, IEntityComponent
                => this.TryQueryByEGID(localId.Value, out value);

        bool TryQueryByLocalId<T>(EntityLocalId localId, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
                => this.TryQueryByEGID(localId.Value, out groupIndex, out value);

        ref T QueryByLocalId<T>(EntityLocalId localId)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(localId.Value);

        ref T QueryByLocalId<T>(EntityLocalId localId, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(localId.Value, out groupIndex);

        ref T QueryByLocalId<T>(EntityLocalId localId, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(localId.Value, out groupIndex, out exists);

        EntityLocalId GetLocalId(EntityGlobalId globalId);
        bool TryGetLocalId(EntityGlobalId globalId, out EntityLocalId localId);

        EntityGlobalId GetGlobalId(EntityLocalId localId);
        bool TryGetGlobalId(EntityLocalId localId, out EntityGlobalId globalId);

        bool TryGetEntity(EntityGlobalId globalId, out Entity entity);
        bool TryGetEntity<T>(EntityGlobalId globalId, out Entity<T> entity)
            where T : unmanaged, IEntityComponent;
        bool TryGetEntity(EntityLocalId localId, out Entity entity);
        bool TryGetEntity<T>(EntityLocalId localId, out Entity<T> entity)
            where T : unmanaged, IEntityComponent;

        bool TryGetEntity(ExclusiveGroupStruct groupId, uint index, out Entity entity);
        bool TryGetEntity<T>(ExclusiveGroupStruct groupId, uint index, out Entity<T> entity)
            where T : unmanaged, IEntityComponent;

        ref T QueryByGroupIndex<T>(GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByGroupIndex<T>(GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent;

        ref T QueryByGroupIndex<T>(ExclusiveGroupStruct groupId, uint index)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByGroupIndex<T>(ExclusiveGroupStruct groupId, uint index, out T value)
            where T : unmanaged, IEntityComponent;

        bool Has<T>(ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent;

        bool Has<T1>(ExclusiveGroupStruct groupId, out EntityCollection<T1> entities)
            where T1 : unmanaged, IEntityComponent;

        bool HasAll<T1, T2>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        bool HasAll<T1, T2, T3>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        bool HasAll<T1, T2, T3, T4>(ExclusiveGroupStruct groupId, out EntityCollection<T1, T2, T3, T4> entities)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        EntityCollection<T1> QueryEntities<T1>(ExclusiveGroupStruct groupId)
            where T1 : unmanaged, IEntityComponent;

        EntityCollection<T1, T2> QueryEntities<T1, T2>(ExclusiveGroupStruct groupId)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        EntityCollection<T1, T2, T3> QueryEntities<T1, T2, T3>(ExclusiveGroupStruct groupId)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        EntityCollection<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(ExclusiveGroupStruct groupId)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1> QueryEntities<T1>()
            where T1 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2> QueryEntities<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1> QueryEntities<T1>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2> QueryEntities<T1, T2>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1>()
            where T1 : unmanaged, IEntityComponent;

        LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        LocalFasterReadOnlyList<ExclusiveGroupStruct> FindGroups<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        int CalculateTotal<T>()
            where T : unmanaged, IEntityComponent;

        bool IsSpawned(GroupIndex groupIndex);
        bool IsSpawned(EGID egid);
        bool IsSpawned(EGID egid, out GroupIndex groupIndex);
        bool IsSpawned(EntityLocalId localId)
            => this.IsSpawned(localId.Value);
        bool IsSpawned(EntityLocalId localId, out GroupIndex groupIndex)
            => this.IsSpawned(localId.Value, out groupIndex);
        bool IsSpawned(EntityGlobalId globalId)
        {
            if (this.TryGetLocalId(globalId, out EntityLocalId localId) == false)
            {
                return false;
            }

            return this.IsSpawned(localId.Value);
        }
        bool IsSpawned(EntityGlobalId globalId, out GroupIndex groupIndex)
        {
            if (this.TryGetLocalId(globalId, out EntityLocalId localId) == false)
            {
                groupIndex = default;
                return false;
            }

            return this.IsSpawned(localId.Value, out groupIndex);
        }

        bool IsDespawned(GroupIndex groupIndex);
        bool IsDespawned(EGID egid);
        bool IsDespawned(EGID egid, out GroupIndex groupIndex);
        bool IsDespawned(EntityLocalId localId)
            => this.IsDespawned(localId.Value);
        bool IsDespawned(EntityLocalId localId, out GroupIndex groupIndex)
            => this.IsDespawned(localId.Value, out groupIndex);
        bool IsDespawned(EntityGlobalId globalId)
            => this.IsDespawned(this.GetLocalId(globalId).Value);
        bool IsDespawned(EntityGlobalId globalId, out GroupIndex groupIndex)
            => this.IsDespawned(this.GetLocalId(globalId).Value, out groupIndex);


        ref EntityFilterCollection GetFilter<T>(EGID egid, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent;

        ref EntityFilterCollection GetFilter<T>(EntityLocalId localId, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(localId.Value, filterContext);
        ref EntityFilterCollection GetFilter<T, TFilter>(EntityLocalId localId)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(localId.Value, FilterContextHelper.GetFilterContext<T, TFilter>());
        ref EntityFilterCollection GetFilter<T, TFilter>(EGID egid)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(egid, FilterContextHelper.GetFilterContext<T, TFilter>());
        ref EntityFilterCollection GetFilter<T>(EntityLocalId localId)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(localId, FilterContextHelper.GetFilterContext<T, EGID>());

        ref EntityFilterCollection GetFilter<T>(CombinedFilterID combinedFilterId)
            where T : unmanaged, IEntityComponent;
        ref EntityFilterCollection GetFilter<T>(int filterId, FilterContextID contextID)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(new CombinedFilterID(filterId, contextID));
        ref EntityFilterCollection GetFilter<T, TFilter>(int filterId)
            where T : unmanaged, IEntityComponent
                => ref this.GetFilter<T>(filterId, FilterContextHelper.GetFilterContext<T, TFilter>());

        ref EntityFilterCollection GetFilter<T>(EntityFilterId<T> filterId)
            where T : unmanaged, IEntityComponent;
    }
}
