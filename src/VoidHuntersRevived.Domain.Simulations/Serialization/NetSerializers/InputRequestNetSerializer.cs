using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Providers;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class InputRequestNetSerializer : NetSerializer<InputRequest>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override InputRequest Deserialize(NetDataReader reader)
        {
            return new InputRequest((SimulationInput)_serializers.Deserialize(reader));
        }

        public override void Serialize(NetDataWriter writer, in InputRequest instance)
        {
            _serializers.Serialize(writer, instance.Input);
        }
    }
}
