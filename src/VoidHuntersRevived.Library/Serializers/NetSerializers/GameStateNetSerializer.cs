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
            var nextTickId = reader.GetUInt();

            var count = reader.GetInt();
            var history = new List<Tick>(count);

            for (var i=0; i<count; i++)
            {
                history.Add(serializers.Deserialize<Tick>(reader));
            }

            return new GameState(
                nextTickId: nextTickId, 
                history: history);
        }

        public override void Serialize(NetDataWriter writer, INetSerializerProvider serializers, in GameState instance)
        {
            writer.Put(instance.NextTickId);

            // writer a placeholder uint that will contain the tick history count
            var poition = writer.Length;
            writer.Put(int.MinValue);

            int count = 0;
            foreach(Tick tick in instance.History)
            {
                if(tick.Count() == 0)
                {
                    continue;
                }

                count++;
                serializers.Serialize<Tick>(writer, false, in tick);
            }

            var endPosition = writer.Length;
            writer.SetPosition(poition);
            writer.Put(count);
            writer.SetPosition(endPosition);
        }
    }
}
