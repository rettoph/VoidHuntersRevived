using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;

namespace LiteNetLib.Utils
{
    public static class BinaryWriterExtensions
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

        public static void Put(this NetDataWriter writer, VhId value)
        {
            writer.Put(value.Value.ToByteArray());
        }
    }
}
