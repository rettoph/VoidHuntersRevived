using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal class TractorBeamEmitter_Activate_NetSerializer : NetSerializer<TractorBeamEmitter_TryActivate>
    {
        public override TractorBeamEmitter_TryActivate Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_TryActivate()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_TryActivate instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
        }
    }
}
