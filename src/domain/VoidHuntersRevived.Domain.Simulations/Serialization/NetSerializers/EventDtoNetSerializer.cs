using Guppy.Core.Network.Common.Providers;
using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class EventDtoNetSerializer : NetSerializer<EventDto>
    {
        private INetSerializerService _serializers = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            this._serializers = serializers;
        }

        public override EventDto Deserialize(NetDataReader reader)
        {
            return new()
            {
                SourceId = reader.GetVhId(),
                Data = (IEventData)this._serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in EventDto instance)
        {
            writer.Put(instance.SourceId);
            this._serializers.Serialize(writer, instance.Data);
        }
    }
}