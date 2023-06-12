using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
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
                Id = reader.GetGuid(),
                Data = (IEventData)_serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in EventDto instance)
        {
            writer.Put(instance.Id);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
