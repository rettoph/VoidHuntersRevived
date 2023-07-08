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
    internal class SetTractorBeamEmitterActiveNetSerializer : NetSerializer<TractorBeamEmitter_Activate>
    {
        public override TractorBeamEmitter_Activate Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_Activate()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_Activate instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
        }
    }
}
