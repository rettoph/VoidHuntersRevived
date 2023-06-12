using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Game.Common.Enums;
using VoidHuntersRevived.Game.Common.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetHelmDirectionNetSerializer : NetSerializer<SetHelmDirection>
    {
        public override SetHelmDirection Deserialize(NetDataReader reader)
        {
            return new SetHelmDirection()
            {
                PilotId = reader.GetGuid(),
                Which = reader.GetEnum<Direction>(),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetHelmDirection instance)
        {
            writer.Put(instance.PilotId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
