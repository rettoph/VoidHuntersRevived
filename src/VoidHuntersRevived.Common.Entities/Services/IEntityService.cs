using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntityService
    {
        #region Entity Ids
        EntityId GetId(VhId vhid);
        EntityId GetId(EGID egid);
        EntityId GetId(uint entityId, ExclusiveGroupStruct groupId);

        bool TryGetId(VhId vhid, out EntityId id);
        bool TryGetId(EGID egid, out EntityId id);
        bool TryGetId(uint entityId, ExclusiveGroupStruct groupId, out EntityId id);

        EntityState GetState(EntityId id);
        #endregion

        #region Entity Spawning
        EntityId Spawn(IEntityType type, VhId vhid, EntityInitializerDelegate? initializer);
        void Despawn(VhId vhid);
        void Despawn(EGID egid);
        void Despawn(EntityId id);
        #endregion

        #region Serialization
        EntityData Serialize(EntityId id);
        EntityData Serialize(VhId vhid);
        EntityData Serialize(EGID egid);
        EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId);

        void Serialize(EntityId id, EntityWriter writer);
        void Serialize(VhId vhid, EntityWriter writer);
        void Serialize(EGID egid, EntityWriter writer);
        void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer);

        EntityId Deserialize(in VhId seed, EntityData data, EntityInitializerDelegate? initializer, bool confirmed = false);
        EntityId Deserialize(EntityReader reader, EntityInitializerDelegate? initializer);
        #endregion

        #region Filters
        ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent;
        #endregion
    }
}
