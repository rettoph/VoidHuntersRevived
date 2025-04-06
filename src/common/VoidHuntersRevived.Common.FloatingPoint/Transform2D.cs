using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.FloatingPoint
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Transform2D(Complex rotation, Vector2 position)
    {
        public static readonly Transform2D Identity = new(Complex.One, Vector2.Zero);

        [FieldOffset(0)]
        public Complex Rotation = rotation;

        [FieldOffset(8)]
        public Vector2 Position = position;

        public Transform2D(float x, float y, float cos, float sin) : this(new Complex(cos, sin), new Vector2(x, y))
        {

        }
        public Transform2D(float x, float y, float radians) : this(new Complex(radians), new Vector2(x, y))
        {

        }
        public Transform2D(Vector2 position, float radians) : this(new Complex(radians), position)
        {

        }

        public static Transform2D Invert(Transform2D transform)
        {
            float iImaginary = -transform.Rotation.Imaginary;
            float x = -(transform.Position.X * transform.Rotation.Real) + (transform.Position.Y * iImaginary);
            float y = -(transform.Position.X * iImaginary) - (transform.Position.Y * transform.Rotation.Real);

            return new Transform2D(x: x, y: y, cos: transform.Rotation.Real, sin: iImaginary);
        }

        public Vector2 TransformVector2(Vector2 target)
        {
            return Transform2D.TransformVector2(target, this);
        }

        public static Vector2 TransformVector2(Vector2 target, Transform2D transform)
        {
            return new Vector2(
                x: (target.X * transform.Rotation.Real) + (target.Y * -transform.Rotation.Imaginary) + transform.Position.X,
                y: (target.X * transform.Rotation.Imaginary) + (target.Y * transform.Rotation.Real) + transform.Position.Y);
        }

        public static Transform2D operator *(Transform2D left, Transform2D right)
        {
            return new Transform2D(
                x: (left.Position.X * right.Rotation.Real) - (left.Position.Y * right.Rotation.Imaginary) + right.Position.X,
                y: (left.Position.X * right.Rotation.Imaginary) + (left.Position.Y * right.Rotation.Real) + right.Position.Y,
                cos: (left.Rotation.Real * right.Rotation.Real) - (left.Rotation.Imaginary * right.Rotation.Imaginary),
                sin: (left.Rotation.Real * right.Rotation.Imaginary) + (left.Rotation.Imaginary * right.Rotation.Real));
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Transform2D d && this == d;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Position, this.Rotation);
        }

        public static bool operator ==(Transform2D left, Transform2D right)
        {
            return left.Position == right.Position &&
                   left.Rotation == right.Rotation;
        }

        public static bool operator !=(Transform2D left, Transform2D right)
        {
            return left.Position != right.Position ||
                   left.Rotation != right.Rotation;
        }

        public static Transform2D CreateRotation(float radians)
        {
            return new(
                x: 0f,
                y: 0f,
                radians: radians);
        }

        public readonly Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;

            result.M11 = this.Rotation.Real;
            result.M12 = this.Rotation.Imaginary;
            result.M21 = -this.Rotation.Imaginary;
            result.M22 = this.Rotation.Real;
            result.M41 = this.Position.X;
            result.M42 = this.Position.Y;

            return result;
        }

        public override readonly string ToString()
        {
            return $"{{ Position = {this.Position}, Rotation = {this.Rotation} }}";
        }
    }
}
