using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static Vector2 GetVector2(this NetDataReader reader)
        {
            return new Vector2()
            {
                X = reader.GetFloat(),
                Y = reader.GetFloat()
            };
        }

        public static void GetVector2(this NetDataReader reader, out Vector2 value)
        {
            value.X = reader.GetFloat();
            value.Y = reader.GetFloat();
        }

        public static void GetParallelKey(this NetDataReader reader, out ParallelKey key)
        {
            key = new ParallelKey(reader.GetUInt128());
        }

        public static ParallelKey GetParallelKey(this NetDataReader reader)
        {
            return new ParallelKey(reader.GetUInt128());
        }
    }
}
