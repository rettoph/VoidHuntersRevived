﻿using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        #region Entity Ids
        EntityId GetId(VhId vhid);

        bool TryGetId(VhId vhid, out EntityId id);
        #endregion

        #region Entity Spawning
        EntityId Spawn(IEntityType type, VhId vhid, TeamId teamId, EntityInitializerDelegate? initializer);
        void Despawn(VhId vhid);
        void Despawn(EntityId id);

        /// <summary>
        /// "Submit" all Svelto changes now. This is automatically done every frame but 
        /// it may be manually done if needed
        /// </summary>
        void Flush();

        bool IsSpawned(EntityId id);
        bool IsSpawned(EntityId id, out GroupIndex groupIndex);
        bool IsSpawned(in GroupIndex groupIndex);
        #endregion

        #region Serialization
        EntityData Serialize(EntityId id);
        EntityData Serialize(VhId vhid);

        void Serialize(EntityId id, EntityWriter writer);
        void Serialize(VhId vhid, EntityWriter writer);

        EntityId Deserialize(in VhId seed, TeamId teamId, EntityData data, EntityInitializerDelegate? initializer, VhId injection = default);
        EntityId Deserialize(EntityReader reader, EntityInitializerDelegate? initializer);
        #endregion

        #region Filters
        ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
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

        public GroupsEnumerable<T1> QueryEntities<T1>()
            where T1 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>()
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1> QueryEntities<T1>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2> QueryEntities<T1, T2>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2, T3> QueryEntities<T1, T2, T3>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent;

        public GroupsEnumerable<T1, T2, T3, T4> QueryEntities<T1, T2, T3, T4>(LocalFasterReadOnlyList<ExclusiveGroupStruct> groups)
            where T1 : unmanaged, IEntityComponent
            where T2 : unmanaged, IEntityComponent
            where T3 : unmanaged, IEntityComponent
            where T4 : unmanaged, IEntityComponent;
        #endregion

        #region Descriptors
        internal IVoidHuntersEntityDescriptorEngine GetDescriptorEngine(VhId descriptorId);
        #endregion
    }
}
