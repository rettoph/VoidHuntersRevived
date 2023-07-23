using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
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
        #endregion

        #region Entity Spawning
        EntityId Spawn(IEntityType Type, VhId VhId, EntityInitializerDelegate? Initializer);
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

        EntityId Deserialize(in VhId seed, EntityData data, bool confirmed);
        EntityId Deserialize(EntityReader reader);
        #endregion
    }
}
