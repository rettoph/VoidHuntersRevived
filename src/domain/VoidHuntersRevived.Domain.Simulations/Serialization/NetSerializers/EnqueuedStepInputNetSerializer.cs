using Guppy.Core.Network.Common.Providers;
using Guppy.Core.Network.Common.Serialization;
using Guppy.Core.Network.Common.Services;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class EnqueuedStepInputNetSerializer : NetSerializer<EnqueuedStepInput>
    {
        private INetSerializerService _serializers = default!;

        public override void Initialize(INetSerializerService serializers)
        {
            base.Initialize(serializers);

            this._serializers = serializers;
        }

        public override EnqueuedStepInput Deserialize(NetDataReader reader)
        {
            return new(reader.GetId<IStepEvent>(), (IStepInput)this._serializers.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in EnqueuedStepInput instance)
        {
            writer.Put(instance.Id);
            this._serializers.Serialize(writer, instance.Data);
        }
    }
}