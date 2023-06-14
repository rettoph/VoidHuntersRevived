using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using System;
using VoidHuntersRevived.Game.Enums;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class SetHelmDirectionNetSerializer : NetSerializer<SetHelmDirection>
    {
        public override SetHelmDirection Deserialize(NetDataReader reader)
        {
            return new SetHelmDirection()
            {
                ShipId = reader.GetVhId(),
                Which = reader.GetEnum<Direction>(),
                Value = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in SetHelmDirection instance)
        {
            writer.Put(instance.ShipId);
            writer.Put(instance.Which);
            writer.Put(instance.Value);
        }
    }
}
