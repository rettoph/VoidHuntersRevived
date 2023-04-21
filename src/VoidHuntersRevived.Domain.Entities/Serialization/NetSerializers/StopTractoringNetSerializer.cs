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
                TargetPosition = reader.GetVector2(),
                TractorableKey = reader.GetParallelKey()
            };
        }

        public override void Serialize(NetDataWriter writer, in StopTractoring instance)
        {
            writer.Put(instance.TargetPosition);
            writer.Put(instance.TractorableKey);
        }
    }
}
