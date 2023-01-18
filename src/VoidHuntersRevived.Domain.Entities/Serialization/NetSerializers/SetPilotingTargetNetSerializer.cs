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
    internal sealed class SetPilotingTargetNetSerializer : NetSerializer<SetPilotingTarget>
    {
        public override SetPilotingTarget Deserialize(NetDataReader reader)
        {
            return new SetPilotingTarget()
            {
                Target = new Vector2(reader.GetFloat(), reader.GetFloat())
            };
        }

        public override void Serialize(NetDataWriter writer, in SetPilotingTarget instance)
        {
            writer.Put(instance.Target.X);
            writer.Put(instance.Target.Y);
        }
    }
}
