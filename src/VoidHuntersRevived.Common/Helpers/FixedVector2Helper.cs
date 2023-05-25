using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Common.Helpers
{
    public static class FixedVector2Helper
    {
        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <returns>Transformed <see cref="Vector2"/>.</returns>
        public static AetherVector2 Transform(AetherVector2 position, FixedMatrix matrix)
        {
            return new AetherVector2(position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41, position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42);
        }

        /// <summary>
        /// Creates a new <see cref="Vector2"/> that contains a transformation of 2d-vector by the specified <see cref="Matrix"/>.
        /// </summary>
        /// <param name="position">Source <see cref="Vector2"/>.</param>
        /// <param name="matrix">The transformation <see cref="Matrix"/>.</param>
        /// <param name="result">Transformed <see cref="Vector2"/> as an output parameter.</param>
        public static void Transform(ref AetherVector2 position, ref FixedMatrix matrix, out AetherVector2 result)
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
            AetherVector2[] sourceArray,
            int sourceIndex,
            ref FixedMatrix matrix,
            AetherVector2[] destinationArray,
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
            AetherVector2[] sourceArray,
            ref FixedMatrix matrix,
            AetherVector2[] destinationArray)
        {
            Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
        }
    }
}
