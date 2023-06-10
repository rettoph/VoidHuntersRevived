using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class DeactivateTractorBeamEmitterNetSerializer : NetSerializer<DeactivateTractorBeamEmitter>
    {
        public override DeactivateTractorBeamEmitter Deserialize(NetDataReader reader)
        {
            return new DeactivateTractorBeamEmitter()
            {
                TractorBeamEmitterKey = reader.GetEventId()
            };
        }

        public override void Serialize(NetDataWriter writer, in DeactivateTractorBeamEmitter instance)
        {
            writer.Put(instance.TractorBeamEmitterKey);
        }
    }
}
