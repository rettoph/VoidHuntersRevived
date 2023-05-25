using FixedMath.NET;
using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

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

        public static Fix64 GetFix64(this NetDataReader reader)
        {
            return Fix64.FromRaw(reader.GetLong());
        }
        public static AetherVector2 GetAetherVector2(this NetDataReader reader)
        {
            return new AetherVector2()
            {
                X = reader.GetFix64(),
                Y = reader.GetFix64()
            };
        }

        public static void GetAetherVector2(this NetDataReader reader, out AetherVector2 value)
        {
            value.X = reader.GetFix64();
            value.Y = reader.GetFix64();
        }
    }
}
