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
using VoidHuntersRevived.Domain.Entities.Enums;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetPilotingDirectionNetSerializer : NetSerializer<SetPilotingDirection>
    {
        public override SetPilotingDirection Deserialize(NetDataReader reader)
        {
            return new SetPilotingDirection(
                pilotKey: ParallelKey.From(ParallelTypes.Pilot, reader.GetInt()),
                which: reader.GetEnum<Direction>(),
                value: reader.GetBool());

        }

        public override void Serialize(NetDataWriter writer, in SetPilotingDirection instance)
        {
            writer.Put(instance.PilotKey.Value);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
