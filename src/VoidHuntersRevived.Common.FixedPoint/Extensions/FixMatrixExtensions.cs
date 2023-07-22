using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.FixedPoint.Extensions
{
    public static class FixMatrixExtensions
    {
        public static FixMatrix Invert(this FixMatrix matrix)
        {
            return FixMatrix.Invert(matrix);
        }

        public static Fix64 Radians(this FixMatrix matrix)
        {
            return Fix64.Atan2(matrix.M21, matrix.M11);
        }
    }
}
