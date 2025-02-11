using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Ships.Common.Events;

namespace VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers
{
    public class Input_TractorBeamEmitter_Select_NetSerializer : NetSerializer<Input_TractorBeamEmitter_Select>
    {
        public override Input_TractorBeamEmitter_Select Deserialize(NetDataReader reader)
        {
            return new()
            {
                TractorBeamEmitterGlobalId = reader.GetEntityGlobalId(),
                TargetNodeGlobalId = reader.GetEntityGlobalId()
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Select instance)
        {
            writer.Put(instance.TractorBeamEmitterGlobalId);
            writer.Put(instance.TargetNodeGlobalId);
        }
    }
}