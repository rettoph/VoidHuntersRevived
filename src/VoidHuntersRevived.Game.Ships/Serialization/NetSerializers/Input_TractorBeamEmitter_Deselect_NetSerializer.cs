using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Serialization.NetSerializers
{
    [AutoLoad]
    internal class Input_TractorBeamEmitter_Deselect_NetSerializer : NetSerializer<Input_TractorBeamEmitter_Deselect>
    {
        public override Input_TractorBeamEmitter_Deselect Deserialize(NetDataReader reader)
        {
            return new Input_TractorBeamEmitter_Deselect()
            {
                ShipVhId = reader.GetVhId(),
                AttachToSocketVhId = reader.GetIf() ? new SocketVhId(reader.GetVhId(), reader.GetByte()) : null
            };
        }

        public override void Serialize(NetDataWriter writer, in Input_TractorBeamEmitter_Deselect instance)
        {
            writer.Put(instance.ShipVhId);

            if (writer.PutIf(instance.AttachToSocketVhId.HasValue))
            {
                writer.Put(instance.AttachToSocketVhId!.Value.NodeVhId);
                writer.Put(instance.AttachToSocketVhId.Value.Index);
            }
        }
    }
}
