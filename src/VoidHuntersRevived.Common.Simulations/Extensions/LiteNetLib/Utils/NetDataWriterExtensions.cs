using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static unsafe void Put(this NetDataWriter writer, in EventId id)
        {
            var test = Guid.NewGuid();
            ulong* longs = (ulong*)&test;
            writer.Put(id.Value);
        }

        public static unsafe void Put(this NetDataWriter writer, EventId id)
        {
            writer.Put(id.Value);
        }
    }
}
