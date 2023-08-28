using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class EventDtoNetSerializer : NetSerializer<EventDto>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override EventDto Deserialize(NetDataReader reader)
        {
            return new EventDto()
            {
                Sender = reader.GetVhId(),
                Data = (IEventData)_serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in EventDto instance)
        {
            writer.Put(instance.Sender);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
