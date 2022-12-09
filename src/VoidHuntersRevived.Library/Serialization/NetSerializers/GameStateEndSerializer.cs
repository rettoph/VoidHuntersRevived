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
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class GameStateEndNetSerializer : NetSerializer<GameStateEnd>
    {
        public override GameStateEnd Deserialize(NetDataReader reader)
        {
            return new GameStateEnd(reader.GetInt());
        }

        public override void Serialize(NetDataWriter writer, in GameStateEnd instance)
        {
            writer.Put(instance.LastTickId);
        }
    }
}
