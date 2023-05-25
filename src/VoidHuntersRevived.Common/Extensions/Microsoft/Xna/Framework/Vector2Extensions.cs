namespace Microsoft.Xna.Framework
{
    public static class Vector2Extensions
    {
        public static Matrix ToTranslation(this Vector2 value, float z = 0)
        {
            return Matrix.CreateTranslation(value.X, value.Y, z);
        }
    }
}
