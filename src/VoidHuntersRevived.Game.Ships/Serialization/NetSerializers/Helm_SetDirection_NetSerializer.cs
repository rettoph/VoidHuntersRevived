﻿using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Common.Pieces.Enums;
using VoidHuntersRevived.Game.Ships.Events;

namespace VoidHuntersRevived.Game.Ships.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class Helm_SetDirection_NetSerializer : NetSerializer<Helm_SetDirection>
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
