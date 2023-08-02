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
    internal class TractorBeamEmitter_TryDeactivate_NetSerializer : NetSerializer<TractorBeamEmitter_TryDeactivate>
    {
        public override TractorBeamEmitter_TryDeactivate Deserialize(NetDataReader reader)
        {
            return new TractorBeamEmitter_TryDeactivate()
            {
                ShipVhId = reader.GetVhId(),
                TargetVhId = reader.GetVhId()
            };
        }

        public override void Serialize(NetDataWriter writer, in TractorBeamEmitter_TryDeactivate instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.TargetVhId);
        }
    }
}
