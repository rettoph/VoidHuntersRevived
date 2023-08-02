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
    internal class TractorBeamEmitter_TryDetach_NetSerializer : NetSerializer<TractorBeamEmitter_TryDetach>
    {
        public override TractorBeamEmitter_TryDetach Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_TryDetach()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_TryDetach instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
        }
    }
}
