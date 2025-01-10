using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Ships.Common.Events;

namespace VoidHuntersRevived.Domain.Ships.Serialization.NetSerializers
{
    internal sealed class Tactical_SetTarget_NetSerialization : NetSerializer<Tactical_SetTarget>
    {
        public override Tactical_SetTarget Deserialize(NetDataReader reader)
        {
            return new()
            {
                ShipGlobalId = reader.GetEntityGlobalId(),
                Value = reader.GetFixVector2(),
                Snap = reader.GetBool()
            };
        }

        public override void Serialize(NetDataWriter writer, in Tactical_SetTarget instance)
        {
            writer.Put(instance.ShipGlobalId);
            writer.Put(instance.Value);
            writer.Put(instance.Snap);
        }
    }
}