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
    internal sealed class Tactical_SetTarget_NetSerialization : NetSerializer<Tactical_SetTarget>
    {
        public override Tactical_SetTarget Deserialize(NetDataReader reader)
        {
            return new Tactical_SetTarget()
            {
                ShipVhId = reader.GetVhId(),
                Value = reader.GetFixVector2()
            };
        }

        public override void Serialize(NetDataWriter writer, in Tactical_SetTarget instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.Value);
        }
    }
}
