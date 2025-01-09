using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Ships.Common.Events;

namespace VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers
{
    internal class Input_TractorBeamEmitter_Deselect_NetSerializer : NetSerializer<Input_TractorBeamEmitter_Deselect>
    {
        public override Input_TractorBeamEmitter_Deselect Deserialize(NetDataReader reader)
        {
            return new Input_TractorBeamEmitter_Deselect()
            {
                TractorBeamEmitterGlobalId = reader.GetEntityGlobalId(),
                AttachToNodeSocketGlobalId = reader.GetIf() ? new NodeSocketGlobalId(reader.GetEntityGlobalId(), reader.GetByte()) : null
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Deselect instance)
        {
            writer.Put(instance.TractorBeamEmitterGlobalId);

            if (writer.PutIf(instance.AttachToNodeSocketGlobalId.HasValue))
            {
                writer.Put(instance.AttachToNodeSocketGlobalId!.Value.NodeGlobalId);
                writer.Put(instance.AttachToNodeSocketGlobalId.Value.SocketIndex);
            }
        }
    }
}