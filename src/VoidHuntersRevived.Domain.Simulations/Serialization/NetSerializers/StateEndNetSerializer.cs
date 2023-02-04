using Guppy.Attributes;
using Guppy.Attributes.Common;
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
    internal sealed class StateEndNetSerializer : NetSerializer<StateEnd>
    {
        public override StateEnd Deserialize(NetDataReader reader)
        {
            return new StateEnd()
            {
                LastTickId = reader.GetInt()
            };
        }

        public override void Serialize(NetDataWriter writer, in StateEnd instance)
        {
            writer.Put(instance.LastTickId);
        }
    }
}
