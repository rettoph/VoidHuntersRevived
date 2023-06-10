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
    }
}
