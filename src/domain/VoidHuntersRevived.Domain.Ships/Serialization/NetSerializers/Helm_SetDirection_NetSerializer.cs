using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Pieces.Common.Enums;
using VoidHuntersRevived.Domain.Ships.Common.Events;

namespace VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers
{
    internal sealed class Helm_SetDirection_NetSerializer : NetSerializer<Helm_SetDirection>
    {
        public override Helm_SetDirection Deserialize(NetDataReader reader)
        {
            return new Helm_SetDirection()
            {
                ShipGlobalId = reader.GetEntityGlobalId(),
                Which = reader.GetEnum<DirectionEnum>(),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in Helm_SetDirection instance)
        {
            writer.Put(instance.ShipGlobalId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}