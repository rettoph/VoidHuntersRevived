using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    public class Simulation_Begin_NetSerializer : NetSerializer<SimulationBegin>
    {
        public override SimulationBegin Deserialize(NetDataReader reader)
        {
            return new();
        }

        public override void Serialize(NetDataWriter writer, in SimulationBegin instance)
        {
        }
    }
}