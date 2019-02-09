using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Helpers
{
    public static class RadianHelper
    {
        public static readonly Single TWO_PI = (float)(Math.PI * 2);
        public static readonly Single PI = (float)Math.PI;
        public static readonly Single PI_HALVES = ((float)Math.PI) / 2;
        public static readonly Single THREE_PI_HALVES = ((float)(3 * Math.PI)) / 2;

        public static Single Normalize(Single theta)
        {
            Single normalized = theta % TWO_PI;
            normalized = (normalized + TWO_PI) % TWO_PI;
            //return normalized <= Math.PI ? normalized : normalized - TWO_PI;
            return normalized;
        }
    }
}
