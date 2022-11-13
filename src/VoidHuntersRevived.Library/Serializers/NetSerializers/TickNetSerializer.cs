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
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override Tick Deserialize(NetDataReader reader)
        {
            var id = reader.GetInt();
            var count = reader.GetByte();

            if(count == 0)
            {
                return new Tick(id, Enumerable.Empty<ITickData>());
            }

            var items = new List<ITickData>(count);

            for (var i = 0; i < count; i++)
            {
                if(_serializers.Deserialize(reader) is ITickData data)
                {
                    items.Add(data);
                }
            }

            var instance = new Tick(id, items);

            return instance;
        }

        public override void Serialize(NetDataWriter writer, in Tick instance)
        {
            writer.Put(instance.Id);

            var count = (byte)instance.Count();

            writer.Put(count);

            foreach (ITickData data in instance)
            {
                _serializers.Serialize(writer, data);
            }
        }
    }
}
