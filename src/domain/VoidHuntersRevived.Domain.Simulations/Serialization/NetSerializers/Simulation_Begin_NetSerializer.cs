using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Events;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal class Simulation_Begin_NetSerializer : NetSerializer<Simulation_Begin>
    {
        public override Simulation_Begin Deserialize(NetDataReader reader) => new();

        public override void Serialize(NetDataWriter writer, in Simulation_Begin instance)
        {
        }
    }
}