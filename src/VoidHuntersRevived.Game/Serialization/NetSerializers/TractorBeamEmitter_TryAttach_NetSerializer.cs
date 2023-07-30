using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal class TractorBeamEmitter_TryAttach_NetSerializer : NetSerializer<TractorBeamEmitter_TryAttach>
    {
        public override TractorBeamEmitter_TryAttach Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_TryAttach()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId(),
                SocketVhId = new SocketVhId(reader.GetVhId(), reader.GetByte())
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_TryAttach instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
            writer.Put(instance.SocketVhId.NodeVhId);
            writer.Put(instance.SocketVhId.Index);
        }
    }
}
