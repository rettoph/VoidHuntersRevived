using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal class Simulation_Begin_NetSerializer : NetSerializer<Simulation_Begin>
    {
        public override Simulation_Begin Deserialize(NetDataReader reader)
        {
            return new Simulation_Begin();
        }

        public override void Serialize(NetDataWriter writer, in Simulation_Begin instance)
        {
        }
    }
}
