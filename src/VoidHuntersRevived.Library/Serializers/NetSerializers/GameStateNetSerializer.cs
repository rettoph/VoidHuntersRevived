using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Definitions;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serializers.NetSerializers
{
    [AutoLoad]
    internal sealed class GameStateNetSerializer : NetSerializer<GameState>
    {
        public override GameState Deserialize(NetDataReader reader, INetSerializerProvider serializers)
        {
            return new GameState()
            {
                NextTickId = reader.GetUInt()
            };
        }

        public override void Serialize(NetDataWriter writer, INetSerializerProvider serializers, in GameState instance)
        {
            writer.Put(instance.NextTickId);
        }
    }
}
