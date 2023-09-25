using Autofac.Features.Metadata;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Common
{
    public struct FixVector2
    {
        public static FixVector2 Zero = new FixVector2(Fix64.Zero, Fix64.Zero);
        public static FixVector2 One = new FixVector2(Fix64.One, Fix64.One);
        public static FixVector2 UnitX = new FixVector2(Fix64.One, Fix64.Zero);
        public static FixVector2 UnitY = new FixVector2(Fix64.Zero, Fix64.One);

        public Fix64 X;
        public Fix64 Y;

        public Fix64 Length => Fix64.Sqrt(this.X * this.X + this.Y * this.Y);

        public FixVector2(decimal x, decimal y) : this((Fix64)x, (Fix64)y)
        {

        }
        public FixVector2(Fix64 x, Fix64 y)
        {
            this.X = x;
            this.Y = y;
        }

        public static void Distance(ref FixVector2 v1, ref FixVector2 v2, out Fix64 result)
        {
            Fix64 dx = v1.X - v2.X;
            Fix64 dy = v1.Y - v2.Y;
            result = Fix64.Sqrt(dx * dx + dy * dy);
        }

        public static Fix64 Distance(FixVector2 v1, FixVector2 v2)
        {
            Fix64 dx = v1.X - v2.X;
            Fix64 dy = v1.Y - v2.Y;
            return Fix64.Sqrt(dx * dx + dy * dy);
        }

        public FixPolar ToPolar()
        {
            return new FixPolar(
                length: this.Length,
                radians: Fix64.Atan2(this.X, this.Y));
        }

        /// <summary>
        /// Creates a new <see cref="AetherVector2"/> that contains linear interpolation of the specified vectors.
        /// </summary>
        /// <param name="value1">The first vector.</param>
        /// <param name="value2">The second vector.</param>
        /// <param name="amount">Weighting value(between 0.0 and 1.0).</param>
        /// <returns>The result of linear interpolation of the specified vectors.</returns>
        public static FixVector2 Lerp(FixVector2 v1, FixVector2 v2, Fix64 amount)
        {
            return new FixVector2(
                x: Fix64.Lerp(v1.X, v2.X, amount),
                y: Fix64.Lerp(v1.Y, v2.Y, amount));
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static FixVector2 Transform(FixVector2 position, FixMatrix matrix)
        {
            return new FixVector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">Transformed <see cref="Vector2"/> as an output parameter.</param>
        public static void Transform(ref FixVector2 position, ref FixMatrix matrix, out FixVector2 result)
        {
            var x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
            var y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
            result.X = x;
            result.Y = y;
        }

        /// <summary>
        /// Apply transformation on vectors within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
        /// </summary>
        /// <param name="sourceArray">Source array.</param>
        /// <param name="sourceIndex">The starting index of transformation in the source array.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destinationArray">Destination array.</param>
        /// <param name="destinationIndex">The starting index in the destination array, where the first <see cref="Vector2"/> should be written.</param>
        /// <param name="length">The number of vectors to be transformed.</param>
        public static void Transform(
            FixVector2[] sourceArray,
            int sourceIndex,
            ref FixMatrix matrix,
            FixVector2[] destinationArray,
            int destinationIndex,
            int length)
        {
            if (sourceArray == null)
                throw new ArgumentNullException("sourceArray");
            if (destinationArray == null)
                throw new ArgumentNullException("destinationArray");
            if (sourceArray.Length < sourceIndex + length)
                throw new ArgumentException("Source array length is lesser than sourceIndex + length");
            if (destinationArray.Length < destinationIndex + length)
                throw new ArgumentException("Destination array length is lesser than destinationIndex + length");

            for (int x = 0; x < length; x++)
            {
                var position = sourceArray[sourceIndex + x];
                var destination = destinationArray[destinationIndex + x];
                destination.X = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
                destination.Y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
                destinationArray[destinationIndex + x] = destination;
            }
        }

        /// <summary>
        /// Apply transformation on all vectors within array of <see cref="Vector2"/> by the specified <see cref="Matrix"/> and places the results in an another array.
        /// </summary>
        /// <param name="sourceArray">Source array.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="destinationArray">Destination array.</param>
        public static void Transform(
            FixVector2[] sourceArray,
            ref FixMatrix matrix,
            FixVector2[] destinationArray)
        {
            Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
        }

        public static FixVector2 operator +(FixVector2 left, FixVector2 right)
        {
            left.X += right.X;
            left.Y += right.Y;
            return left;
        }

        public static FixVector2 operator -(FixVector2 left, FixVector2 right)
        {
            left.X -= right.X;
            left.Y -= right.Y;
            return left;
        }

        public static FixVector2 operator -(FixVector2 right)
        {
            right.X = -right.X;
            right.Y = -right.Y;
            return right;
        }

        public static FixVector2 operator *(FixVector2 left, FixVector2 right)
        {
            left.X *= right.X;
            left.Y *= right.Y;
            return left;
        }

        public static FixVector2 operator *(FixVector2 left, Fix64 right)
        {
            left.X *= right;
            left.Y *= right;
            return left;
        }

        public static FixVector2 operator *(Fix64 left, FixVector2 right)
        {
            right.X *= left;
            right.Y *= left;
            return right;
        }

        public static FixVector2 operator /(FixVector2 left, Fix64 right)
        {
            Fix64 invRight = Fix64.One / right;
            left.X *= invRight;
            left.Y *= invRight;
            return left;
        }

        public static bool operator ==(FixVector2 left, FixVector2 right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(FixVector2 left, FixVector2 right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        public static explicit operator Vector2(FixVector2 fixedVector2)
        {
            return new Vector2((float)fixedVector2.X, (float)fixedVector2.Y);
        }

        public static explicit operator FixVector2(Vector2 vector2)
        {
            return new FixVector2((Fix64)vector2.X, (Fix64)vector2.Y);
        }

        public static FixVector2 FromPolar(Fix64 length, Fix64 radians)
        {
            return new FixVector2(
                x: Fix64.Cos(radians) * length,
                y: Fix64.Sin(radians) * length);
        }

        public static FixVector2 Rotate(FixVector2 vector2, Fix64 radians)
        {
            return FixVector2.FromPolar(vector2.Length, Fix64.Atan2(vector2.X, vector2.Y) + radians);
        }
    }
}
