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
            return Fix64.Atan2(matrix.M12, matrix.M11);
        }

        public static BoundingSphere GetBoudingSphere(this FixMatrix matrix, float radius)
        {
            return new BoundingSphere(new Vector3((float)matrix.M41, (float)matrix.M42, (float)matrix.M43), radius);
        }
    }
}
