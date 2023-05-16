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
    internal sealed class SetHelmDirectionNetSerializer : NetSerializer<SetHelmDirection>
    {
        public override SetHelmDirection Deserialize(NetDataReader reader)
        {
            return new SetHelmDirection()
            {
                HelmKey = reader.GetParallelKey(),
                Which = reader.GetEnum<Direction>(),
                Value = reader.GetBool()
            };

        }

        public override void Serialize(NetDataWriter writer, in SetHelmDirection instance)
        {
            writer.Put(instance.HelmKey);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
