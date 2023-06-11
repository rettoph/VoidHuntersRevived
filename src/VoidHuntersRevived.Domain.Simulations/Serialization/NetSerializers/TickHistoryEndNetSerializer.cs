using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class TickHistoryEndNetSerializer : NetSerializer<TickHistoryEnd>
    {
        public override TickHistoryEnd Deserialize(NetDataReader reader)
        {
            return new TickHistoryEnd()
            {
                CurrentTickId = reader.GetInt()
            };
        }

        public override void Serialize(NetDataWriter writer, in TickHistoryEnd instance)
        {
            writer.Put(instance.CurrentTickId);
        }
    }
}
