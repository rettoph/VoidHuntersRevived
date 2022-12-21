using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad(0)]
    internal sealed class DirectionInputSerializer : NetSerializer<DirectionInput>
    {
        public override DirectionInput Deserialize(NetDataReader reader)
        {
            return new DirectionInput(
                which: reader.GetEnum<Direction>(),
                value: reader.GetBool());
        }

        public override void Serialize(NetDataWriter writer, in DirectionInput instance)
        {
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
