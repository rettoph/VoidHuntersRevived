using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
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

        EntityId GetId(VhId vhid);

        bool TryGetId(VhId vhid, out EntityId id);

        public bool TryQueryById<T>(EntityId id, out T value)
            where T : unmanaged, IEntityComponent
                => this.TryQueryByEGID(id.EGID, out value);

        public bool TryQueryById<T>(EntityId id, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
                => this.TryQueryByEGID(id.EGID, out groupIndex, out value);

        public ref T QueryById<T>(EntityId id)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(id.EGID);

        public ref T QueryById<T>(EntityId id, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(id.EGID, out groupIndex);

        public ref T QueryById<T>(EntityId id, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent
                => ref this.QueryByEGID<T>(id.EGID, out groupIndex, out exists);

        /// <summary>
        /// Warning, extra lookup, less efficient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vhid"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryQueryByVhId<T>(VhId vhid, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (this.TryGetId(vhid, out EntityId id) && this.TryQueryById(id, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Warning, extra lookup, less efficient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="vhid"></param>
        /// <param name="groupIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryQueryByVhId<T>(VhId vhid, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent
        {
            if (this.TryGetId(vhid, out EntityId id) && this.TryQueryById(id, out groupIndex, out value))
            {
                return true;
            }

            groupIndex = default;
            value = default;
            return false;
        }

        /// <summary>
        /// Warning, extra lookup, even less efficient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        ref T QueryByVhId<T>(VhId vhid)
            where T : unmanaged, IEntityComponent => ref this.QueryById<T>(this.GetId(vhid));

        /// <summary>
        /// Warning, extra lookup, even less efficient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="groupIndex"></param>
        /// <returns></returns>
        ref T QueryByVhId<T>(VhId vhid, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent => ref this.QueryById<T>(this.GetId(vhid), out groupIndex);

        /// <summary>
        /// Warning, extra lookup, even less efficient
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="groupIndex"></param>
        /// <param name="exists"></param>
        /// <returns></returns>
        ref T QueryByVhId<T>(VhId vhid, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent => ref this.QueryById<T>(this.GetId(vhid), out groupIndex, out exists);

        ref T QueryByGroupIndex<T>(in GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByGroupIndex<T>(in GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent;

        ref T QueryByGroupIndex<T>(ExclusiveGroupStruct groupId, uint index)
            where T : unmanaged, IEntityComponent;

        bool TryQueryByGroupIndex<T>(in ExclusiveGroupStruct groupId, uint index, out T value)
            where T : unmanaged, IEntityComponent;

        bool HasAny<T>(ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent;

        bool HasAny<T1>(ExclusiveGroupStruct groupId, out EntityCollection<T1> entities)
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

        bool IsSpawned(EntityId id);
        bool IsSpawned(EntityId id, out GroupIndex groupIndex);
        bool IsSpawned(in GroupIndex groupIndex);

        bool IsDespawned(EntityId id);
        bool IsDespawned(EntityId id, out GroupIndex groupIndex);
        bool IsDespawned(in GroupIndex groupIndex);

        ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent;

        ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent;

        ref EntityFilterCollection GetFilter<T>(EntityFilterId<T> filterId)
            where T : unmanaged, IEntityComponent;
    }
}
