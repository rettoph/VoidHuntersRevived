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
                AttachToSocketVhId = reader.GetIf() ? new SocketVhId(reader.GetVhId(), reader.GetByte()) : null
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Deselect instance)
        {
            writer.Put(instance.TractorBeamEmitterGlobalId);

            if (writer.PutIf(instance.AttachToSocketVhId.HasValue))
            {
                writer.Put(instance.AttachToSocketVhId!.Value.NodeVhId);
                writer.Put(instance.AttachToSocketVhId.Value.Index);
            }
        }
    }
}
