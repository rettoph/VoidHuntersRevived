using LiteNetLib.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
using VoidHuntersRevived.Common;

namespace LiteNetLib.Utils
{
    public static class NetDataReaderExtensions
    {
        public static Fix64 GetFix64(this NetDataReader reader)
        {
            return Fix64.FromRaw(reader.GetLong());
        }
        public static FixVector2 GetFixVector2(this NetDataReader reader)
        {
            return new FixVector2()
            {
                X = reader.GetFix64(),
                Y = reader.GetFix64()
            };
        }

        public static void GetFixVector2(this NetDataReader reader, out AetherVector2 value)
        {
            value.X = reader.GetFix64();
            value.Y = reader.GetFix64();
        }
    }
}
