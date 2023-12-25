using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Simulations.Lockstep;

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
            var hash = reader.GetVhId();
            var id = reader.GetInt();
            var queue = reader.GetEnum<TickQueue>();
            var count = reader.GetByte();
            Tick tick = default!;

            if (count == 0)
            {
                tick = Tick.Empty(id, queue);

                if (tick.Hash != hash)
                {
                    throw new Exception();
                }

                return tick;
            }

            var items = new EventDto[count];

            for (var i = 0; i < count; i++)
            {
                if (_serializers.Deserialize(reader) is EventDto input)
                {
                    items[i] = input;
                }
            }

            tick = Tick.Create(id, items, queue);
            if (tick.Hash != hash)
            {
                throw new Exception();
            }

            return tick;
        }

        public override void Serialize(NetDataWriter writer, in Tick instance)
        {
            writer.Put(instance.Hash);
            writer.Put(instance.Id);
            writer.Put(instance.Queue);

            var count = (byte)instance.Events.Length;

            writer.Put(count);

            foreach (EventDto input in instance.Events)
            {
                _serializers.Serialize(writer, input);
            }
        }
    }
}
