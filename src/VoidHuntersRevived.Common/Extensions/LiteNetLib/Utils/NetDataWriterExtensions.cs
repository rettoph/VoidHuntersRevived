using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataWriterExtensions
    {
        public static void Put(this NetDataWriter writer, Vector2 value)
        {
            writer.Put(value.X);
            writer.Put(value.Y);
        }

        public static void Put(this NetDataWriter writer, in Vector2 value)
        {
            writer.Put(value.X);
            writer.Put(value.Y);
        }

        public static unsafe void Put(this NetDataWriter writer, in ParallelKey key)
        {
            var test = Guid.NewGuid();
            ulong* longs = (ulong*)&test;
            writer.Put(key.Value);
        }

        public static unsafe void Put(this NetDataWriter writer, ParallelKey key)
        {
            writer.Put(key.Value);
        }
    }
}
