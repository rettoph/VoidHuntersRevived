using Guppy.Attributes;
using Guppy.Network;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    [AutoLoad]
    internal sealed class TickHistoryStartNetSerializer : NetSerializer<TickHistoryStart>
    {
        public override TickHistoryStart Deserialize(NetDataReader reader)
        {
            return new TickHistoryStart()
            {
                CurrentTickId = reader.GetInt()
            };
        }

        public override void Serialize(NetDataWriter writer, in TickHistoryStart instance)
        {
            writer.Put(instance.CurrentTickId);
        }
    }
}
