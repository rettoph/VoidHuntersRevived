using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Extensions.System
{
    public static class RandomExtensions
    {
        public static Single NextSingle(this Random rand, Single min, Single max)
        {
            return (Single)((rand.NextDouble() * (max - min)) + min);
        }

        public static Vector2 NextVector2(this Random rand, Single min, Single max)
        {
            return rand.NextVector2(min, max, min, max);
        }
        public static Vector2 NextVector2(this Random rand, Single minX, Single maxX, Single minY, Single maxY)
        {
            return new Vector2(rand.NextSingle(minX, maxX), rand.NextSingle(minY, maxY));
        }
    }
}
