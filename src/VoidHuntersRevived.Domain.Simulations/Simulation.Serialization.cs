using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Simulations
{
    public partial class Simulation
    {
        public EntityData Serialize(IdMap id)
        {
            return this.Engines.Serialization.Serialize(id);
        }
        public EntityData Serialize(VhId vhid)
        {
            return this.Serialize(this.Entities.GetIdMap(vhid));
        }
        public EntityData Serialize(EGID egid)
        {
            return this.Serialize(this.Entities.GetIdMap(egid));
        }
        public EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId)
        {
            return this.Serialize(this.Entities.GetIdMap(entityId, groupId));
        }
        public void Serialize(IdMap id, EntityWriter writer)
        {
            this.Engines.Serialization.Serialize(id, writer);
        }
        public void Serialize(VhId vhid, EntityWriter writer)
        {
            this.Serialize(this.Entities.GetIdMap(vhid), writer);
        }
        public void Serialize(EGID egid, EntityWriter writer)
        {
            this.Serialize(this.Entities.GetIdMap(egid), writer);
        }
        public void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer)
        {
            this.Serialize(this.Entities.GetIdMap(entityId, groupId), writer);
        }
        public IdMap Deserialize(VhId seed, EntityData data)
        {
            return this.Engines.Serialization.Deserialize(seed, data);
        }
        public IdMap Deserialize(EntityReader reader)
        {
            return this.Engines.Serialization.Deserialize(reader);
        }
    }
}
