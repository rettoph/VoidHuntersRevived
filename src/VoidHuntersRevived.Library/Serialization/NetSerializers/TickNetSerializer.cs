﻿using Guppy.Attributes;
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
    [AutoLoad(0)]
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
                return new Tick(id, Enumerable.Empty<ISimulationEvent>());
            }

            var items = new List<ISimulationEvent>(count);

            for (var i = 0; i < count; i++)
            {
                if(_serializers.Deserialize(reader) is ISimulationEvent data)
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

            var count = (byte)instance.Events.Count();

            writer.Put(count);

            foreach (ISimulationEvent data in instance.Events)
            {
                _serializers.Serialize(writer, data);
            }
        }
    }
}
