using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class TickHistoryItemNetSerializer : NetSerializer<TickHistoryItem>
    {
        private INetSerializer<Tick> _serializer = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializer = serializers.Get<Tick>();
        }

        public override TickHistoryItem Deserialize(NetDataReader reader)
        {
            return new TickHistoryItem()
            {
                Tick = _serializer.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in TickHistoryItem instance)
        {
            _serializer.Serialize(writer, instance.Tick);
        }
    }
}
