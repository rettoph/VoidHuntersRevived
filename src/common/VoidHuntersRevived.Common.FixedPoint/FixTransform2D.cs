using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixTransform2D(FixComplex rotation, FixVector2 position)
    {
        public static readonly FixTransform2D Identity = new(FixComplex.One, FixVector2.Zero);

        public FixComplex Rotation = rotation;

        public FixVector2 Position = position;

        public FixTransform2D(Fix64 x, Fix64 y, Fix64 cos, Fix64 sin) : this(new FixComplex(cos, sin), new FixVector2(x, y))
        {

        }
        public FixTransform2D(Fix64 x, Fix64 y, Fix64 radians) : this(new FixComplex(radians), new FixVector2(x, y))
        {

        }
        public FixTransform2D(FixVector2 position, Fix64 radians) : this(new FixComplex(radians), position)
        {

        }

        public static FixTransform2D Invert(FixTransform2D transform)
        {
            Fix64 iImaginary = -transform.Rotation.Imaginary;
            Fix64 x = -(transform.Position.X * transform.Rotation.Real) + (transform.Position.Y * iImaginary);
            Fix64 y = -(transform.Position.X * iImaginary) - (transform.Position.Y * transform.Rotation.Real);

            return new FixTransform2D(x: x, y: y, cos: transform.Rotation.Real, sin: iImaginary);
        }

        public static FixTransform2D operator *(FixTransform2D left, FixTransform2D right)
        {
            return new FixTransform2D(
                x: (left.Position.X * right.Rotation.Real) - (left.Position.Y * right.Rotation.Imaginary) + right.Position.X,
                y: (left.Position.X * right.Rotation.Imaginary) + (left.Position.Y * right.Rotation.Real) + right.Position.Y,
                cos: (left.Rotation.Real * right.Rotation.Real) - (left.Rotation.Imaginary * right.Rotation.Imaginary),
                sin: (left.Rotation.Real * right.Rotation.Imaginary) + (left.Rotation.Imaginary * right.Rotation.Real));
        }

        public static FixTransform2D operator *(FixTransform2D left, FixVector2 right)
        {
            return new FixTransform2D(
                x: left.Position.X + right.X,
                y: left.Position.Y + right.Y,
                cos: left.Rotation.Real,
                sin: left.Rotation.Imaginary);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is FixTransform2D d && this == d;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Position, this.Rotation);
        }

        public static bool operator ==(FixTransform2D left, FixTransform2D right)
        {
            return left.Position == right.Position &&
                   left.Rotation == right.Rotation;
        }

        public static bool operator !=(FixTransform2D left, FixTransform2D right)
        {
            return left.Position != right.Position ||
                   left.Rotation != right.Rotation;
        }

        public static FixTransform2D CreateRotation(Fix64 radians)
        {
            return new(
                x: Fix64.Zero,
                y: Fix64.Zero,
                radians: radians);
        }

        public readonly FixMatrix ToFixMatrix()
        {
            FixMatrix result = FixMatrix.Identity;

            result.M11 = this.Rotation.Real;
            result.M12 = this.Rotation.Imaginary;
            result.M21 = -this.Rotation.Imaginary;
            result.M22 = this.Rotation.Real;
            result.M41 = this.Position.X;
            result.M42 = this.Position.Y;

            return result;
        }

        public readonly Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;

            result.M11 = (float)this.Rotation.Real;
            result.M12 = (float)this.Rotation.Imaginary;
            result.M21 = -result.M12;
            result.M22 = result.M11;
            result.M41 = (float)this.Position.X;
            result.M42 = (float)this.Position.Y;

            return result;
        }

        public override readonly string ToString()
        {
            return $"{{ Position = {this.Position}, Rotation = {this.Rotation} }}";
        }
    }
}