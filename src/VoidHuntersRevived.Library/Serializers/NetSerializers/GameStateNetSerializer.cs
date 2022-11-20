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

namespace VoidHuntersRevived.Library.Serializers.NetSerializers
{
    [AutoLoad]
    internal sealed class GameStateNetSerializer : NetSerializer<GameState>
    {
        public override GameState Deserialize(NetDataReader reader)
        {
            GameStateType type = reader.GetEnum<GameStateType>();

            if(type == GameStateType.Begin)
            {
                return GameState.Begin(reader.GetInt());
            }

            return GameState.End;
        }

        public override void Serialize(NetDataWriter writer, in GameState instance)
        {
            writer.Put(instance.Type);

            if(instance.Type == GameStateType.Begin)
            {
                writer.Put(instance.LastHistoricTickId);
            }
        }
    }
}
