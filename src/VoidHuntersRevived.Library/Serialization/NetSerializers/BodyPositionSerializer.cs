using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal class BodyPositionSerializer : NetSerializer<BodyPosition>
    {
        public override BodyPosition Deserialize(NetDataReader reader)
        {
            return new BodyPosition(
                id: reader.GetInt(),
                position: new Vector2()
                {
                    X = reader.GetFloat(),
                    Y = reader.GetFloat(),
                });
        }

        public override void Serialize(NetDataWriter writer, in BodyPosition instance)
        {
            writer.Put(instance.Id);

            writer.Put(instance.Position.X);
            writer.Put(instance.Position.Y);
        }
    }
}
