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
        EntityData Serialize(VhId vhid);
        IdMap Deserialize(VhId seed, EntityData data);
    }
}
