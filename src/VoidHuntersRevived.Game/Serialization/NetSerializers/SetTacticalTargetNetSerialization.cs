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
    internal sealed class SetTacticalTargetNetSerialization : NetSerializer<SetTacticalTarget>
    {
        public override SetTacticalTarget Deserialize(NetDataReader reader)
        {
            return new SetTacticalTarget()
            {
                ShipId = reader.GetVhId(),
                Value = reader.GetFixVector2()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetTacticalTarget instance)
        {
            writer.Put(instance.ShipId);
            writer.Put(instance.Value);
        }
    }
}
