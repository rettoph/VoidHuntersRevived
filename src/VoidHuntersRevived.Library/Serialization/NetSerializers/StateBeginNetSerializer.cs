using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
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
