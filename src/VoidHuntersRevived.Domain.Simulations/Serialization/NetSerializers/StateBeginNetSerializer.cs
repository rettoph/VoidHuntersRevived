using Guppy.Attributes;
using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StateBeginNetSerializer : NetSerializer<StateBegin>
    {
        public override StateBegin Deserialize(NetDataReader reader)
        {
            return new StateBegin();
        }

        public override void Serialize(NetDataWriter writer, in StateBegin instance)
        {
        }
    }
}
