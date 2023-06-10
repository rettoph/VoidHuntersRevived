using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Events;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetTacticalTargetNetSerializer : NetSerializer<SetTacticalTarget>
    {
        public override SetTacticalTarget Deserialize(NetDataReader reader)
        {
            return new SetTacticalTarget()
            {
                TacticalKey = reader.GetEventId(),
                Target = reader.GetFixVector2()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetTacticalTarget instance)
        {
            writer.Put(instance.TacticalKey);
            writer.Put(instance.Target);
        }
    }
}
