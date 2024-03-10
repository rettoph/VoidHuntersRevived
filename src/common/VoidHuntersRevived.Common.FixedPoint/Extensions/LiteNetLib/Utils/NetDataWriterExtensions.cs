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

        public static void Put(this NetDataWriter writer, Fix64 value)
        {
            writer.Put(value.RawValue);
        }

        public static void Put(this NetDataWriter writer, FixVector2 value)
        {
            writer.Put(value.X);
            writer.Put(value.Y);
        }

        public static void Put(this NetDataWriter writer, in FixVector2 value)
        {
            writer.Put(value.X);
            writer.Put(value.Y);
        }
    }
}
