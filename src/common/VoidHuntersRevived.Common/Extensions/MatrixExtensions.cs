using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.Extensions
{
    public static class MatrixExtensions
    {
        public static void Deconstruct(this Matrix matrix, out float x, out float y, out float cos, out float sin)
        {
            x = matrix.M41;
            y = matrix.M42;
            cos = matrix.M11;
            sin = matrix.M12;
        }
    }
}