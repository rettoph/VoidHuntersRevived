﻿using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntitySerializationService
    {
        EntityData Serialize(IdMap id);
        EntityData Serialize(VhId vhid);
        EntityData Serialize(EGID egid);
        EntityData Serialize(uint entityId, ExclusiveGroupStruct groupId);
        void Serialize(IdMap id, EntityWriter writer);
        void Serialize(VhId vhid, EntityWriter writer);
        void Serialize(EGID egid, EntityWriter writer);
        void Serialize(uint entityId, ExclusiveGroupStruct groupId, EntityWriter writer);
        IdMap Deserialize(VhId seed, EntityData data);
        IdMap Deserialize(EntityData data);
        IdMap Deserialize(EntityReader reader);
    }
}
