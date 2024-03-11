using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface IEntityService
    {
        #region Entity Ids
        EntityId GetId(VhId vhid);

        bool TryGetId(VhId vhid, out EntityId id);
        #endregion

        #region Entity Spawning
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, Id<ITeam> teamId);
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, Id<ITeam> teamId, EntityInitializerDelegate initializer);
        EntityId Spawn(VhId sourceId, IEntityType type, VhId vhid, Id<ITeam> teamId, InstanceEntityInitializerDelegate initializer);

        void Despawn(VhId sourceId, VhId vhid);
        void Despawn(VhId sourceId, EntityId id);

        /// <summary>
        /// "Submit" all Svelto changes now. This is automatically done every frame but 
        /// it may be manually done if needed
        /// </summary>
        void Flush();

        bool IsSpawned(EntityId id);
        bool IsSpawned(EntityId id, out GroupIndex groupIndex);
        bool IsSpawned(in GroupIndex groupIndex);

        bool IsDespawned(EntityId id);
        bool IsDespawned(EntityId id, out GroupIndex groupIndex);
        bool IsDespawned(in GroupIndex groupIndex);
        #endregion

        #region Serialization
        EntityData Serialize(EntityId id, SerializationOptions options);

        EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data);
        EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, EntityInitializerDelegate initializer);
        EntityId Deserialize(VhId sourceId, DeserializationOptions options, EntityData data, InstanceEntityInitializerDelegate initializer);
        #endregion

        #region Filters
        ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent;

        ref EntityFilterCollection GetFilter<T>(CombinedFilterID filterId)
            where T : unmanaged, IEntityComponent;
        #endregion

        #region Querying
        bool TryQueryById<T>(EntityId id, out T value)
            where T : unmanaged, IEntityComponent;

        bool TryQueryById<T>(EntityId id, out GroupIndex groupIndex, out T value)
            where T : unmanaged, IEntityComponent;

        ref T QueryById<T>(EntityId id)
            where T : unmanaged, IEntityComponent;

        ref T QueryById<T>(EntityId id, out GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent;

        ref T QueryById<T>(EntityId id, out GroupIndex groupIndex, out bool exists)
            where T : unmanaged, IEntityComponent;

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
        #endregion

        #region Descriptors
        internal IVoidHuntersEntityDescriptorEngine GetDescriptorEngine(Id<VoidHuntersEntityDescriptor> descriptorId);
        #endregion
    }
}
