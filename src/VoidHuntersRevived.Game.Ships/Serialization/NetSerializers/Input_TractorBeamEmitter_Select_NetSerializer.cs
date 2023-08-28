using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Serialization.NetSerializers
{
    [AutoLoad]
    internal class Input_TractorBeamEmitter_Select_NetSerializer : NetSerializer<Input_TractorBeamEmitter_Select>
    {
        public override Input_TractorBeamEmitter_Select Deserialize(NetDataReader reader)
        {
            return new Input_TractorBeamEmitter_Select()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Select instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
        }
    }
}
