using Guppy.Core.Network.Common.Providers;
using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    internal sealed class TickNetSerializer : NetSerializer<Tick>
    {
        private INetSerializerService _serializers = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            this._serializers = serializers;
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

            for (int i = 0; i < count; i++)
            {
                if (this._serializers.Deserialize(reader) is EventDto input)
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
                this._serializers.Serialize(writer, input);
            }
        }
    }
}