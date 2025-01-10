using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class TickHistoryItemNetSerializer : NetSerializer<TickHistoryItem>
    {
        private INetSerializer<Tick> _serializer = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            this._serializer = serializers.Get<Tick>();
        }

        public override TickHistoryItem Deserialize(NetDataReader reader) => new()
        {
            Tick = this._serializer.Deserialize(reader)
        };

        public override void Serialize(NetDataWriter writer, in TickHistoryItem instance) => this._serializer.Serialize(writer, instance.Tick);
    }
}