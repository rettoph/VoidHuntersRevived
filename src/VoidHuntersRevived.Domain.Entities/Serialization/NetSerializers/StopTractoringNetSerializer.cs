using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StopTractoringNetSerializer : NetSerializer<StopTractoring>
    {
        public override StopTractoring Deserialize(NetDataReader reader)
        {
            return new StopTractoring()
            {
                Key = reader.GetParallelKey(),
                SenderId = reader.GetInt(),
                TargetPosition = reader.GetVector2(),
                TargetTreeKey = reader.GetParallelKey()
            };
        }

        public override void Serialize(NetDataWriter writer, in StopTractoring instance)
        {
            writer.Put(instance.Key);
            writer.Put(instance.SenderId);
            writer.Put(instance.TargetPosition);
            writer.Put(instance.TargetTreeKey);
        }
    }
}
