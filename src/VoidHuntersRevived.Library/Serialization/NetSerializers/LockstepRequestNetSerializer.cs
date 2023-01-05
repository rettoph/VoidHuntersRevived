using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Library.Common;

namespace VoidHuntersRevived.Library.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class LockstepRequestNetSerializer : NetSerializer<Lockstep.ClientRequest>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override Lockstep.ClientRequest Deserialize(NetDataReader reader)
        {
            return new Lockstep.ClientRequest((ISimulationData)_serializers.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in Lockstep.ClientRequest instance)
        {
            _serializers.Serialize(writer, instance.Data);
        }
    }
}
