using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetHelmDirectionNetSerializer : NetSerializer<Helm_SetDirection>
    {
        public override Helm_SetDirection Deserialize(NetDataReader reader)
        {
            return new Helm_SetDirection()
            {
                ShipVhId = reader.GetVhId(),
                Which = reader.GetEnum<Direction>(),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in Helm_SetDirection instance)
        {
            writer.Put(instance.ShipVhId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
