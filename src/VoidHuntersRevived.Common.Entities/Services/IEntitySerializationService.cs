using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Serialization;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IEntitySerializationService
    {
        EntityData Serialize(IdMap id);
        void Serialize(IdMap id, EntityWriter writer);

        IdMap Deserialize(VhId seed, EntityData data, EventValidity validity);
        IdMap Deserialize(EntityData data, EventValidity validity);
        IdMap Deserialize(EntityReader reader, EventValidity validity);
    }
}
