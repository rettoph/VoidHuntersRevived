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
    internal class SetTractorBeamEmitterActiveNetSerializer : NetSerializer<SetTractorBeamEmitterActive>
    {
        public override SetTractorBeamEmitterActive Deserialize(NetDataReader reader)
        {
            return new SetTractorBeamEmitterActive()
            {
                ShipId = reader.GetVhId(),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetTractorBeamEmitterActive instance)
        {
            writer.Put(instance.ShipId);
            writer.Put(instance.Value);
        }
    }
}
