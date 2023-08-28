using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Serialization.NetSerializers
{
    [AutoLoad]
    internal class Input_TractorBeamEmitter_Deselect_NetSerializer : NetSerializer<Input_TractorBeamEmitter_Deselect>
    {
        public override Input_TractorBeamEmitter_Deselect Deserialize(NetDataReader reader)
        {
            return new Input_TractorBeamEmitter_Deselect()
            {
                ShipVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Deselect instance)
        {
            writer.Put(instance.ShipVhId);
        }
    }
}
