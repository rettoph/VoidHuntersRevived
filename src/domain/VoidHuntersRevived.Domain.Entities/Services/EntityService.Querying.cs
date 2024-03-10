using Svelto.DataStructures;
using Svelto.ECS;
using System.Runtime.CompilerServices;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
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
    }
}
