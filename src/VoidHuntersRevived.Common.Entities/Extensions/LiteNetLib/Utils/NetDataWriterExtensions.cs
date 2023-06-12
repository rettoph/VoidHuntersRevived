using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static unsafe void Put(this NetDataWriter writer, in EntityId id)
        {
            writer.Put(id.Value);
        }

        public static unsafe void Put(this NetDataWriter writer, EntityId id)
        {
            writer.Put(id.Value);
        }
    }
}
