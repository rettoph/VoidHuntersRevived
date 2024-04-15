using Guppy.Core.Common.Attributes;
using Guppy.Core.Network;
using Guppy.Core.Network.Providers;
using Guppy.Core.Network.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class EventDtoNetSerializer : NetSerializer<EventDto>
    {
        private INetSerializerService _serializers = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override EventDto Deserialize(NetDataReader reader)
        {
            return new EventDto()
            {
                SourceId = reader.GetVhId(),
                Data = (IEventData)_serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in EventDto instance)
        {
            writer.Put(instance.SourceId);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
