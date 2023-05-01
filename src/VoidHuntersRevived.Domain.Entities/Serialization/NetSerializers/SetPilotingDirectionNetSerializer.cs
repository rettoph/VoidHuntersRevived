using Guppy.Attributes;
using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Messages;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetPilotingDirectionNetSerializer : NetSerializer<SetPilotingDirection>
    {
        public override SetPilotingDirection Deserialize(NetDataReader reader)
        {
            return new SetPilotingDirection()
            {
                Key = reader.GetParallelKey(),
                SenderId = reader.GetInt(),
                Which = reader.GetEnum<Direction>(),
                Value = reader.GetBool()
            };

        }

        public override void Serialize(NetDataWriter writer, in SetPilotingDirection instance)
        {
            writer.Put(instance.Key);
            writer.Put(instance.SenderId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
