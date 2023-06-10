using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class ActivateTractorBeamEmitterNetSerializer : NetSerializer<ActivateTractorBeamEmitter>
    {
        public override ActivateTractorBeamEmitter Deserialize(NetDataReader reader)
        {
            return new ActivateTractorBeamEmitter()
            {
                TractorBeamEmitterKey = reader.GetEventId()
            };
        }

        public override void Serialize(NetDataWriter writer, in ActivateTractorBeamEmitter instance)
        {
            writer.Put(instance.TractorBeamEmitterKey);
        }
    }
}
