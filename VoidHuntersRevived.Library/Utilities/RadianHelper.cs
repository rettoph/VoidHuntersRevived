using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Utilities
{
    public static class RadianHelper
    {
        public static Single Normalize(Single theta)
        {
            Single normalized = theta % MathHelper.TwoPi;
            normalized = (normalized + MathHelper.TwoPi) % MathHelper.TwoPi;
            //return normalized <= Math.PI ? normalized : normalized - TWO_PI;
            return normalized;
        }

        public static Single FromDeg(Single deg)
        {
            return deg * (MathHelper.Pi / 180);
        }
    }
}
