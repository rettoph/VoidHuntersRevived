using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixTransform2D(Fix64 x, Fix64 y, Fix64 cos, Fix64 sin)
    {
        public Fix64 X = x;
        public Fix64 Y = y;

        public Fix64 Cos = cos;
        public Fix64 Sin = sin;

        public Fix64 Rotation
        {
            get
            {
                if (this.Sin == Fix64.Zero && this.Cos == Fix64.One)
                {
                    return Fix64.Zero;
                }

                return Fix64.Atan2(this.Sin, this.Cos);
            }
            set
            {
                this.Cos = Fix64.Cos(value);
                this.Sin = Fix64.Sin(value);
            }
        }

        public FixTransform2D(Fix64 x, Fix64 y, Fix64 rotation) : this(x, y, Fix64.Cos(rotation), Fix64.Sin(rotation))
        {

        }
        public FixTransform2D(FixVector2 position, Fix64 rotation) : this(position.X, position.Y, Fix64.Cos(rotation), Fix64.Sin(rotation))
        {

        }

        public static readonly FixTransform2D Identity = new(
            x: Fix64.Zero, y: Fix64.Zero,
            cos: Fix64.One, sin: Fix64.Zero);

        public static FixTransform2D Invert(FixTransform2D transform)
        {
            Fix64 iSin = -transform.Sin;
            Fix64 x = -(transform.X * transform.Cos) + (transform.Y * iSin);
            Fix64 y = -(transform.X * iSin) - (transform.Y * transform.Cos);

            return new FixTransform2D(x: x, y: y, cos: transform.Cos, sin: iSin);
        }

        public static FixTransform2D operator *(FixTransform2D left, FixTransform2D right)
        {
            return new FixTransform2D(
                x: (left.X * right.Cos) - (left.Y * right.Sin) + right.X,
                y: (left.X * right.Sin) + (left.Y * right.Cos) + right.Y,
                cos: (left.Cos * right.Cos) - (left.Sin * right.Sin),
                sin: (left.Cos * right.Sin) + (left.Sin * right.Cos));
        }

        public static FixTransform2D operator *(FixTransform2D left, FixVector2 right)
        {
            return new FixTransform2D(
                x: left.X + right.X,
                y: left.Y + right.Y,
                cos: left.Cos,
                sin: left.Sin);
        }

        public override bool Equals(object? obj)
        {
            return obj is FixTransform2D d && this == d;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.X, this.Y, this.Sin, this.Cos);
        }

        public static bool operator ==(FixTransform2D left, FixTransform2D right)
        {
            return left.X == right.X &&
                   left.Y == right.Y &&
                   left.Sin == right.Sin &&
                   left.Cos == right.Cos;
        }

        public static bool operator !=(FixTransform2D left, FixTransform2D right)
        {
            return left.X != right.X ||
                   left.Y != right.Y ||
                   left.Sin != right.Sin ||
                   left.Cos != right.Cos;
        }

        public static FixTransform2D CreateRotation(Fix64 rotation)
        {
            return new FixTransform2D(
                x: Fix64.Zero,
                y: Fix64.Zero,
                cos: Fix64.Cos(rotation),
                sin: Fix64.Sin(rotation));
        }

        public FixMatrix ToFixMatrix()
        {
            FixMatrix result = FixMatrix.Identity;

            result.M11 = this.Cos;
            result.M12 = this.Sin;
            result.M21 = -this.Sin;
            result.M22 = this.Cos;
            result.M41 = this.X;
            result.M42 = this.Y;

            return result;
        }

        public Matrix ToMatrix()
        {
            Matrix result = Matrix.Identity;

            result.M11 = (float)this.Cos;
            result.M12 = (float)this.Sin;
            result.M21 = -result.M12;
            result.M22 = result.M11;
            result.M41 = (float)this.X;
            result.M42 = (float)this.Y;

            return result;
        }
    }
}
