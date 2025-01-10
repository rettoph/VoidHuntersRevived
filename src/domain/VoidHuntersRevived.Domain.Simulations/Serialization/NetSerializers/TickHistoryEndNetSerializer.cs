using Guppy.Core.Network.Common.Serialization;
using LiteNetLib.Utils;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Serialization.NetSerializers
{
    internal sealed class TickHistoryEndNetSerializer : NetSerializer<TickHistoryEnd>
    {
        public override TickHistoryEnd Deserialize(NetDataReader reader) => new()
        {
            CurrentTickId = reader.GetInt()
        };

        public override void Serialize(NetDataWriter writer, in TickHistoryEnd instance) => writer.Put(instance.CurrentTickId);
    }
}