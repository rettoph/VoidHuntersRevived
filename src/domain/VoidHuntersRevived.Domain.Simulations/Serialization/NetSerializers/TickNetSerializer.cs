using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Providers;
using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class TickNetSerializer : NetSerializer<Tick>
    {
        private INetSerializerService _serializers = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override Tick Deserialize(NetDataReader reader)
        {
            VhId hash = reader.GetVhId();
            int id = reader.GetInt();
            byte count = reader.GetByte();
            Tick tick = default!;

            if (count == 0)
            {
                tick = Tick.Empty(id);

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

            tick = Tick.Create(id, items);
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

            byte count = (byte)instance.Events.Length;
            writer.Put(count);
            foreach (EventDto input in instance.Events)
            {
                _serializers.Serialize(writer, input);
            }
        }
    }
}
