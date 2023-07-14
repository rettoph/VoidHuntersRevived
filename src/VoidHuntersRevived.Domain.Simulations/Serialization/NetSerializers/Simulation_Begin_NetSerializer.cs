using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Events;

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
