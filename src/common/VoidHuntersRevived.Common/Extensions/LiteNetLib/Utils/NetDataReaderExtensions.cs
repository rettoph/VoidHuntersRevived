using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Core;

namespace LiteNetLib.Utils
{
    public static class BinaryReaderExtensions
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

        public static VhId GetVhId(this NetDataReader reader)
        {
            byte[] buffer = new byte[16];
            reader.GetBytes(buffer, 16);

            return new VhId(new Guid(buffer));
        }
    }
}
