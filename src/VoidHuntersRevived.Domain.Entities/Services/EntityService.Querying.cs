using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

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
            if(this.entitiesDB.TryQueryEntitiesAndIndex<T>(id.EGID, out uint index, out var components))
            {
                value = components[index];
                groupIndex = new GroupIndex(id.EGID.groupID, index);
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

        public ref T QueryByGroupIndex<T>(in GroupIndex groupIndex)
            where T : unmanaged, IEntityComponent
        {
            var (entities, _) = this.entitiesDB.QueryEntities<T>(groupIndex.GroupID);

            return ref entities[groupIndex.Index];
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

        public bool HasAny<T>(ExclusiveGroupStruct groupID)
            where T : unmanaged, IEntityComponent
        {
            return this.entitiesDB.HasAny<T>(groupID);
        }
    }
}
