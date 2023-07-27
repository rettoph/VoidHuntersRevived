using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Xna.Framework
{
    public static class MatrixExtensions
    {
        public static Matrix Invert(this Matrix matrix)
        {
            return Matrix.Invert(matrix);
        }

        public static float Radians(this Matrix matrix)
        {
            return (float)Math.Atan2(matrix.M12, matrix.M11);
        }
    }
}
