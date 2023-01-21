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
    internal sealed class LockstepRequestNetSerializer : NetSerializer<ClientRequest>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override ClientRequest Deserialize(NetDataReader reader)
        {
            return new ClientRequest(
                user: ParallelTypes.Pilot.ReadValue(reader),
                data: (IData)_serializers.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in ClientRequest instance)
        {
            instance.User.WriteValue(writer);
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
