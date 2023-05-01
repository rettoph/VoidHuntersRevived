using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
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
                return Tick.Empty(id);
            }

            var items = new SimulationEventData[count];

            for (var i = 0; i < count; i++)
            {
                if(_serializers.Deserialize(reader) is SimulationEventData input)
                {
                    items[i] = input;
                }
            }

            var instance = Tick.Create(id, items);

            return instance;
        }

        public override void Serialize(NetDataWriter writer, in Tick instance)
        {
            writer.Put(instance.Id);

            var count = (byte)instance.Count;

            writer.Put(count);

            foreach (SimulationEventData input in instance.Events)
            {
                _serializers.Serialize(writer, input);
            }
        }
    }
}
