using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Definitions;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal sealed class SimulationStateEndNetSerializer : NetSerializer<SimulationStateEnd>
    {
        public override SimulationStateEnd Deserialize(NetDataReader reader)
        {
            return new SimulationStateEnd(reader.GetInt());
        }

        public override void Serialize(NetDataWriter writer, in SimulationStateEnd instance)
        {
            writer.Put(instance.LastTickId);
        }
    }
}
