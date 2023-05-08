using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class StopTractoringNetSerializer : NetSerializer<TryStopTractoring>
    {
        public override TryStopTractoring Deserialize(NetDataReader reader)
        {
            return new TryStopTractoring()
            {
                EmitterKey = reader.GetParallelKey()
            };
        }

        public override void Serialize(NetDataWriter writer, in TryStopTractoring instance)
        {
            writer.Put(instance.EmitterKey);
        }
    }
}
