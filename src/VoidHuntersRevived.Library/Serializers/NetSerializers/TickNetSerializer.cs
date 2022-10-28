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
    internal sealed class TickNetSerializer : NetSerializer<Tick>
    {
        private TickBuffer _buffer;

        public TickNetSerializer(TickBuffer buffer)
        {
            _buffer = buffer;
        }

        public override Tick Deserialize(NetDataReader reader, INetSerializerProvider serializers)
        {
            var id = _buffer.DecompressId(reader.GetByte());
            var count = reader.GetByte();

            if(count == 0)
            {
                return new Tick(id, Enumerable.Empty<ITickData>());
            }

            var items = new List<ITickData>(count);

            for (var i = 0; i < count; i++)
            {
                if(serializers.Deserialize(reader) is ITickData data)
                {
                    items.Add(data);
                }
            }

            var instance = new Tick(id, items);

            return instance;
        }

        public override void Serialize(NetDataWriter writer, INetSerializerProvider serializers, in Tick instance)
        {
            writer.Put(_buffer.CompressId(instance.Id));

            var count = (byte)instance.Count();

            writer.Put(count);

            foreach (ITickData data in instance)
            {
                serializers.Serialize(writer, data);
            }
        }
    }
}
