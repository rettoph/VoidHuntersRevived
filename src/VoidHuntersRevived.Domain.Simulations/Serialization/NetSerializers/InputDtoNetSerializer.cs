using Guppy.Attributes;
using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class InputDtoNetSerializer : NetSerializer<InputDto>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override InputDto Deserialize(NetDataReader reader)
        {
            return new InputDto()
            {
                Id = reader.GetGuid(),
                Sender = reader.GetParallelKey(),
                Data = (IData)_serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in InputDto instance)
        {
            writer.Put(instance.Id);
            writer.Put(instance.Sender);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
