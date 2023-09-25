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

        public static FixMatrix ToRotationMatrix(this FixMatrix matrix)
        {
            var result = FixMatrix.Identity;
            result.M11 = matrix.M11;
            result.M12 = matrix.M12;
            result.M21 = matrix.M21;
            result.M22 = matrix.M22;

            return result;
        }

        public static BoundingSphere GetBoudingSphere(this FixMatrix matrix, float radius)
        {
            return new BoundingSphere(new Vector3((float)matrix.M41, (float)matrix.M42, (float)matrix.M43), radius);
        }

        public static Matrix ToTransformationXnaMatrix(this FixMatrix matrix)
        {
            float cos = (float)matrix.M11;
            float sin = (float)matrix.M12;
            float x = (float)matrix.M41;
            float y = (float)matrix.M42;
            return new Matrix(
                cos,    sin,    0f,     0f,
                -sin,   cos,    0f,     0f,
                0f,     0f,     1f,     0f,
                x,      y,      0f,     1f
            );
        }
    }
}
