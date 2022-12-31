using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal sealed class PilotDirectionInputSerializer : NetSerializer<PilotDirectionInput>
    {
        public override PilotDirectionInput Deserialize(NetDataReader reader)
        {
            return new PilotDirectionInput(
                pilotId: reader.GetUShort(),
                which: reader.GetEnum<Direction>(),
                value: reader.GetBool());
        }

        public override void Serialize(NetDataWriter writer, in PilotDirectionInput instance)
        {
            writer.Put(instance.PilotId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
