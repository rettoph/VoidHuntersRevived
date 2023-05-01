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
    internal sealed class SimulationEventDataNetSerializer : NetSerializer<SimulationEventData>
    {
        private INetSerializerProvider _serializers = default!;

        public override void Initialize(INetSerializerProvider serializers)
        {
            base.Initialize(serializers);

            _serializers = serializers;
        }

        public override SimulationEventData Deserialize(NetDataReader reader)
        {
            return new SimulationEventData()
            {
                Key = reader.GetParallelKey(),
                SenderId = reader.GetInt(),
                Body = _serializers.Deserialize(reader)
            };
        }

        public override void Serialize(NetDataWriter writer, in SimulationEventData instance)
        {
            writer.Put(instance.Key);
            writer.Put(instance.SenderId);
            _serializers.Serialize(writer, instance.Body);
        }
    }
}
