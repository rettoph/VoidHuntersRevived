using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace tainicom.Aether.Physics2D.Common
{
    public static class AetherVector2Extensions
    {
        public static XnaVector2 ToXnaVector2(this AetherVector2 fixedVector)
        {
            return new XnaVector2(
                x: (float)fixedVector.X,
                y: (float)fixedVector.Y);
        }
    }
}
