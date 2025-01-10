using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class TickHistoryStartNetSerializer : NetSerializer<TickHistoryStart>
    {
        public override TickHistoryStart Deserialize(NetDataReader reader)
        {
            return new()
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