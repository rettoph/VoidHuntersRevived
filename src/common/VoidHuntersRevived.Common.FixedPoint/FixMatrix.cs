using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Common.FixedPoint
{
    /// <summary>
    /// Represents the right-handed 4x4 floating point matrix, which can store translation, scale and rotation information.
    /// </summary>
    /// <remarks>
    /// Constructs a matrix.
    /// </remarks>
    /// <param name="m11">A first row and first column value.</param>
    /// <param name="m12">A first row and second column value.</param>
    /// <param name="m13">A first row and third column value.</param>
    /// <param name="m14">A first row and fourth column value.</param>
    /// <param name="m21">A second row and first column value.</param>
    /// <param name="m22">A second row and second column value.</param>
    /// <param name="m23">A second row and third column value.</param>
    /// <param name="m24">A second row and fourth column value.</param>
    /// <param name="m31">A third row and first column value.</param>
    /// <param name="m32">A third row and second column value.</param>
    /// <param name="m33">A third row and third column value.</param>
    /// <param name="m34">A third row and fourth column value.</param>
    /// <param name="m41">A fourth row and first column value.</param>
    /// <param name="m42">A fourth row and second column value.</param>
    /// <param name="m43">A fourth row and third column value.</param>
    /// <param name="m44">A fourth row and fourth column value.</param>
    public struct FixMatrix(
        Fix64 m11, Fix64 m12, Fix64 m13, Fix64 m14,
        Fix64 m21, Fix64 m22, Fix64 m23, Fix64 m24,
        Fix64 m31, Fix64 m32, Fix64 m33, Fix64 m34,
        Fix64 m41, Fix64 m42, Fix64 m43, Fix64 m44
    ) : IEquatable<FixMatrix>
    {

        #region Public Constructors

        #endregion

        #region Public Fields

        /// <summary>
        /// A first row and first column value.
        /// </summary>
        public Fix64 M11 = m11;

        /// <summary>
        /// A first row and second column value.
        /// </summary>
        public Fix64 M12 = m12;

        /// <summary>
        /// A first row and third column value.
        /// </summary>
        public Fix64 M13 = m13;

        /// <summary>
        /// A first row and fourth column value.
        /// </summary>
        public Fix64 M14 = m14;

        /// <summary>
        /// A second row and first column value.
        /// </summary>
        public Fix64 M21 = m21;

        /// <summary>
        /// A second row and second column value.
        /// </summary>
        public Fix64 M22 = m22;

        /// <summary>
        /// A second row and third column value.
        /// </summary>
        public Fix64 M23 = m23;

        /// <summary>
        /// A second row and fourth column value.
        /// </summary>
        public Fix64 M24 = m24;

        /// <summary>
        /// A third row and first column value.
        /// </summary>
        public Fix64 M31 = m31;

        /// <summary>
        /// A third row and second column value.
        /// </summary>
        public Fix64 M32 = m32;

        /// <summary>
        /// A third row and third column value.
        /// </summary>
        public Fix64 M33 = m33;

        /// <summary>
        /// A third row and fourth column value.
        /// </summary>
        public Fix64 M34 = m34;

        /// <summary>
        /// A fourth row and first column value.
        /// </summary>
        public Fix64 M41 = m41;

        /// <summary>
        /// A fourth row and second column value.
        /// </summary>
        public Fix64 M42 = m42;

        /// <summary>
        /// A fourth row and third column value.
        /// </summary>
        public Fix64 M43 = m43;

        /// <summary>
        /// A fourth row and fourth column value.
        /// </summary>
        public Fix64 M44 = m44;

        #endregion

        #region Indexers

        /// <summary>
        /// Get or set the matrix element at the given index, indexed in row major order.
        /// </summary>
        /// <param name="index">The linearized, zero-based index of the matrix element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the index is less than <code>0</code> or larger than <code>15</code>.
        /// </exception>
        public Fix64 this[int index]
        {
            readonly get
            {
                return index switch
                {
                    0 => this.M11,
                    1 => this.M12,
                    2 => this.M13,
                    3 => this.M14,
                    4 => this.M21,
                    5 => this.M22,
                    6 => this.M23,
                    7 => this.M24,
                    8 => this.M31,
                    9 => this.M32,
                    10 => this.M33,
                    11 => this.M34,
                    12 => this.M41,
                    13 => this.M42,
                    14 => this.M43,
                    15 => this.M44,
                    _ => throw new ArgumentOutOfRangeException(nameof(index)),
                };
            }

            set
            {
                switch (index)
                {
                    case 0: this.M11 = value; break;
                    case 1: this.M12 = value; break;
                    case 2: this.M13 = value; break;
                    case 3: this.M14 = value; break;
                    case 4: this.M21 = value; break;
                    case 5: this.M22 = value; break;
                    case 6: this.M23 = value; break;
                    case 7: this.M24 = value; break;
                    case 8: this.M31 = value; break;
                    case 9: this.M32 = value; break;
                    case 10: this.M33 = value; break;
                    case 11: this.M34 = value; break;
                    case 12: this.M41 = value; break;
                    case 13: this.M42 = value; break;
                    case 14: this.M43 = value; break;
                    case 15: this.M44 = value; break;
                    default: throw new ArgumentOutOfRangeException(nameof(index));
                }
            }
        }

        /// <summary>
        /// Get or set the value at the specified row and column (indices are zero-based).
        /// </summary>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the row or column is less than <code>0</code> or larger than <code>3</code>.
        /// </exception>
        public Fix64 this[int row, int column]
        {
            readonly get
            {
                return this[(row * 4) + column];
            }

            set
            {
                this[(row * 4) + column] = value;
            }
        }

        #endregion

        #region Private Members
        private static FixMatrix _identity = new(
            m11: Fix64.One, m12: Fix64.Zero, m13: Fix64.Zero, m14: Fix64.Zero,
            m21: Fix64.Zero, m22: Fix64.One, m23: Fix64.Zero, m24: Fix64.Zero,
            m31: Fix64.Zero, m32: Fix64.Zero, m33: Fix64.One, m34: Fix64.Zero,
            m41: Fix64.Zero, m42: Fix64.Zero, m43: Fix64.Zero, m44: Fix64.One
        );
        #endregion
        /// <summary>
        /// Returns the identity matrix.
        /// </summary>
        public static FixMatrix Identity
        {
            get => FixMatrix._identity;
        }


        #region Public Methods

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="matrix1">The first matrix to add.</param>
        /// <param name="matrix2">The second matrix to add.</param>
        /// <returns>The result of the matrix addition.</returns>
        public static FixMatrix Add(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> which contains sum of two matrixes.
        /// </summary>
        /// <param name="matrix1">The first matrix to add.</param>
        /// <param name="matrix2">The second matrix to add.</param>
        /// <param name="result">The result of the matrix addition as an output parameter.</param>
        public static void Add(ref FixMatrix matrix1, ref FixMatrix matrix2, out FixMatrix result)
        {
            result.M11 = matrix1.M11 + matrix2.M11;
            result.M12 = matrix1.M12 + matrix2.M12;
            result.M13 = matrix1.M13 + matrix2.M13;
            result.M14 = matrix1.M14 + matrix2.M14;
            result.M21 = matrix1.M21 + matrix2.M21;
            result.M22 = matrix1.M22 + matrix2.M22;
            result.M23 = matrix1.M23 + matrix2.M23;
            result.M24 = matrix1.M24 + matrix2.M24;
            result.M31 = matrix1.M31 + matrix2.M31;
            result.M32 = matrix1.M32 + matrix2.M32;
            result.M33 = matrix1.M33 + matrix2.M33;
            result.M34 = matrix1.M34 + matrix2.M34;
            result.M41 = matrix1.M41 + matrix2.M41;
            result.M42 = matrix1.M42 + matrix2.M42;
            result.M43 = matrix1.M43 + matrix2.M43;
            result.M44 = matrix1.M44 + matrix2.M44;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="FixMatrix"/> around X axis.</returns>
        public static FixMatrix CreateRotationX(Fix64 radians)
        {
            FixMatrix.CreateRotationX(radians, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around X axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="FixMatrix"/> around X axis as an output parameter.</param>
        public static void CreateRotationX(Fix64 radians, out FixMatrix result)
        {
            result = FixMatrix.Identity;

            var val1 = Fix64.Cos(radians);
            var val2 = Fix64.Sin(radians);

            result.M22 = val1;
            result.M23 = val2;
            result.M32 = -val2;
            result.M33 = val1;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="FixMatrix"/> around Y axis.</returns>
        public static FixMatrix CreateRotationY(Fix64 radians)
        {
            FixMatrix.CreateRotationY(radians, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around Y axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="FixMatrix"/> around Y axis as an output parameter.</param>
        public static void CreateRotationY(Fix64 radians, out FixMatrix result)
        {
            result = FixMatrix.Identity;

            Fix64 val1 = Fix64.Cos(radians);
            Fix64 val2 = Fix64.Sin(radians);

            result.M11 = val1;
            result.M13 = -val2;
            result.M31 = val2;
            result.M33 = val1;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <returns>The rotation <see cref="FixMatrix"/> around Z axis.</returns>
        public static FixMatrix CreateRotationZ(Fix64 radians)
        {
            FixMatrix.CreateRotationZ(radians, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new rotation <see cref="FixMatrix"/> around Z axis.
        /// </summary>
        /// <param name="radians">Angle in radians.</param>
        /// <param name="result">The rotation <see cref="FixMatrix"/> around Z axis as an output parameter.</param>
        public static void CreateRotationZ(Fix64 radians, out FixMatrix result)
        {
            result = FixMatrix.Identity;

            Fix64 val1 = Fix64.Cos(radians);
            Fix64 val2 = Fix64.Sin(radians);

            result.M11 = val1;
            result.M12 = val2;
            result.M21 = -val2;
            result.M22 = val1;
        }

        /// <summary>
        /// Creates a new translation <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="xPosition">X coordinate of translation.</param>
        /// <param name="yPosition">Y coordinate of translation.</param>
        /// <param name="zPosition">Z coordinate of translation.</param>
        /// <returns>The translation <see cref="FixMatrix"/>.</returns>
        public static FixMatrix CreateTranslation(Fix64 xPosition, Fix64 yPosition, Fix64 zPosition)
        {
            FixMatrix.CreateTranslation(xPosition, yPosition, zPosition, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new translation <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <param name="result">The translation <see cref="FixMatrix"/> as an output parameter.</param>
        public static void CreateTranslation(ref FixVector3 position, out FixMatrix result)
        {
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            result.M34 = Fix64.Zero;
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = Fix64.One;
        }

        /// <summary>
        /// Creates a new translation <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="position">X,Y and Z coordinates of translation.</param>
        /// <returns>The translation <see cref="FixMatrix"/>.</returns>
        public static FixMatrix CreateTranslation(FixVector3 position)
        {
            FixMatrix.CreateTranslation(ref position, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new translation <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="xPosition">X coordinate of translation.</param>
        /// <param name="yPosition">Y coordinate of translation.</param>
        /// <param name="zPosition">Z coordinate of translation.</param>
        /// <param name="result">The translation <see cref="FixMatrix"/> as an output parameter.</param>
        public static void CreateTranslation(Fix64 xPosition, Fix64 yPosition, Fix64 zPosition, out FixMatrix result)
        {
            result.M11 = Fix64.One;
            result.M12 = Fix64.Zero;
            result.M13 = Fix64.Zero;
            result.M14 = Fix64.Zero;
            result.M21 = Fix64.Zero;
            result.M22 = Fix64.One;
            result.M23 = Fix64.Zero;
            result.M24 = Fix64.Zero;
            result.M31 = Fix64.Zero;
            result.M32 = Fix64.Zero;
            result.M33 = Fix64.One;
            result.M34 = Fix64.Zero;
            result.M41 = xPosition;
            result.M42 = yPosition;
            result.M43 = zPosition;
            result.M44 = Fix64.One;
        }



        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by the elements of another matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">Divisor <see cref="FixMatrix"/>.</param>
        /// <returns>The result of dividing the matrix.</returns>
        public static FixMatrix Divide(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 /= matrix2.M11;
            matrix1.M12 /= matrix2.M12;
            matrix1.M13 /= matrix2.M13;
            matrix1.M14 /= matrix2.M14;
            matrix1.M21 /= matrix2.M21;
            matrix1.M22 /= matrix2.M22;
            matrix1.M23 /= matrix2.M23;
            matrix1.M24 /= matrix2.M24;
            matrix1.M31 /= matrix2.M31;
            matrix1.M32 /= matrix2.M32;
            matrix1.M33 /= matrix2.M33;
            matrix1.M34 /= matrix2.M34;
            matrix1.M41 /= matrix2.M41;
            matrix1.M42 /= matrix2.M42;
            matrix1.M43 /= matrix2.M43;
            matrix1.M44 /= matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by the elements of another matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">Divisor <see cref="FixMatrix"/>.</param>
        /// <param name="result">The result of dividing the matrix as an output parameter.</param>
        public static void Divide(ref FixMatrix matrix1, ref FixMatrix matrix2, out FixMatrix result)
        {
            result.M11 = matrix1.M11 / matrix2.M11;
            result.M12 = matrix1.M12 / matrix2.M12;
            result.M13 = matrix1.M13 / matrix2.M13;
            result.M14 = matrix1.M14 / matrix2.M14;
            result.M21 = matrix1.M21 / matrix2.M21;
            result.M22 = matrix1.M22 / matrix2.M22;
            result.M23 = matrix1.M23 / matrix2.M23;
            result.M24 = matrix1.M24 / matrix2.M24;
            result.M31 = matrix1.M31 / matrix2.M31;
            result.M32 = matrix1.M32 / matrix2.M32;
            result.M33 = matrix1.M33 / matrix2.M33;
            result.M34 = matrix1.M34 / matrix2.M34;
            result.M41 = matrix1.M41 / matrix2.M41;
            result.M42 = matrix1.M42 / matrix2.M42;
            result.M43 = matrix1.M43 / matrix2.M43;
            result.M44 = matrix1.M44 / matrix2.M44;
        }

        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static FixMatrix Divide(FixMatrix matrix1, Fix64 divider)
        {
            Fix64 num = Fix64.One / divider;
            matrix1.M11 *= num;
            matrix1.M12 *= num;
            matrix1.M13 *= num;
            matrix1.M14 *= num;
            matrix1.M21 *= num;
            matrix1.M22 *= num;
            matrix1.M23 *= num;
            matrix1.M24 *= num;
            matrix1.M31 *= num;
            matrix1.M32 *= num;
            matrix1.M33 *= num;
            matrix1.M34 *= num;
            matrix1.M41 *= num;
            matrix1.M42 *= num;
            matrix1.M43 *= num;
            matrix1.M44 *= num;
            return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="divider">Divisor scalar.</param>
        /// <param name="result">The result of dividing a matrix by a scalar as an output parameter.</param>
        public static void Divide(ref FixMatrix matrix1, Fix64 divider, out FixMatrix result)
        {
            Fix64 num = Fix64.One / divider;
            result.M11 = matrix1.M11 * num;
            result.M12 = matrix1.M12 * num;
            result.M13 = matrix1.M13 * num;
            result.M14 = matrix1.M14 * num;
            result.M21 = matrix1.M21 * num;
            result.M22 = matrix1.M22 * num;
            result.M23 = matrix1.M23 * num;
            result.M24 = matrix1.M24 * num;
            result.M31 = matrix1.M31 * num;
            result.M32 = matrix1.M32 * num;
            result.M33 = matrix1.M33 * num;
            result.M34 = matrix1.M34 * num;
            result.M41 = matrix1.M41 * num;
            result.M42 = matrix1.M42 * num;
            result.M43 = matrix1.M43 * num;
            result.M44 = matrix1.M44 * num;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="FixMatrix"/> without any tolerance.
        /// </summary>
        /// <param name="other">The <see cref="FixMatrix"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public readonly bool Equals(FixMatrix other)
        {
            return this == other;
        }

        /// <summary>
        /// Compares whether current instance is equal to specified <see cref="object"/> without any tolerance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public override readonly bool Equals(object? obj)
        {
            return obj is FixMatrix matrix
                && this == matrix;
        }

        /// <summary>
        /// Gets the hash code of this <see cref="FixMatrix"/>.
        /// </summary>
        /// <returns>Hash code of this <see cref="FixMatrix"/>.</returns>
        public override readonly int GetHashCode()
        {
            return (this.M11.GetHashCode() + this.M12.GetHashCode() + this.M13.GetHashCode() + this.M14.GetHashCode() + this.M21.GetHashCode() + this.M22.GetHashCode() + this.M23.GetHashCode() + this.M24.GetHashCode() + this.M31.GetHashCode() + this.M32.GetHashCode() + this.M33.GetHashCode() + this.M34.GetHashCode() + this.M41.GetHashCode() + this.M42.GetHashCode() + this.M43.GetHashCode() + this.M44.GetHashCode());
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/>.</param>
        /// <returns>The inverted matrix.</returns>
        public static FixMatrix Invert(FixMatrix matrix)
        {
            FixMatrix.Invert(ref matrix, out FixMatrix result);
            return result;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> which contains inversion of the specified matrix. 
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/>.</param>
        /// <param name="result">The inverted matrix as output parameter.</param>
        public static void Invert(ref FixMatrix matrix, out FixMatrix result)
        {
            Fix64 num1 = matrix.M11;
            Fix64 num2 = matrix.M12;
            Fix64 num3 = matrix.M13;
            Fix64 num4 = matrix.M14;
            Fix64 num5 = matrix.M21;
            Fix64 num6 = matrix.M22;
            Fix64 num7 = matrix.M23;
            Fix64 num8 = matrix.M24;
            Fix64 num9 = matrix.M31;
            Fix64 num10 = matrix.M32;
            Fix64 num11 = matrix.M33;
            Fix64 num12 = matrix.M34;
            Fix64 num13 = matrix.M41;
            Fix64 num14 = matrix.M42;
            Fix64 num15 = matrix.M43;
            Fix64 num16 = matrix.M44;
            Fix64 num17 = ((num11 * num16) - (num12 * num15));
            Fix64 num18 = ((num10 * num16) - (num12 * num14));
            Fix64 num19 = ((num10 * num15) - (num11 * num14));
            Fix64 num20 = ((num9 * num16) - (num12 * num13));
            Fix64 num21 = ((num9 * num15) - (num11 * num13));
            Fix64 num22 = ((num9 * num14) - (num10 * num13));
            Fix64 num23 = ((num6 * num17) - (num7 * num18) + (num8 * num19));
            Fix64 num24 = -((num5 * num17) - (num7 * num20) + (num8 * num21));
            Fix64 num25 = ((num5 * num18) - (num6 * num20) + (num8 * num22));
            Fix64 num26 = -((num5 * num19) - (num6 * num21) + (num7 * num22));
            Fix64 num27 = (Fix64.One / ((num1 * num23) + (num2 * num24) + (num3 * num25) + (num4 * num26)));

            result.M11 = num23 * num27;
            result.M21 = num24 * num27;
            result.M31 = num25 * num27;
            result.M41 = num26 * num27;
            result.M12 = -((num2 * num17) - (num3 * num18) + (num4 * num19)) * num27;
            result.M22 = ((num1 * num17) - (num3 * num20) + (num4 * num21)) * num27;
            result.M32 = -((num1 * num18) - (num2 * num20) + (num4 * num22)) * num27;
            result.M42 = ((num1 * num19) - (num2 * num21) + (num3 * num22)) * num27;
            Fix64 num28 = ((num7 * num16) - (num8 * num15));
            Fix64 num29 = ((num6 * num16) - (num8 * num14));
            Fix64 num30 = ((num6 * num15) - (num7 * num14));
            Fix64 num31 = ((num5 * num16) - (num8 * num13));
            Fix64 num32 = ((num5 * num15) - (num7 * num13));
            Fix64 num33 = ((num5 * num14) - (num6 * num13));
            result.M13 = ((num2 * num28) - (num3 * num29) + (num4 * num30)) * num27;
            result.M23 = -((num1 * num28) - (num3 * num31) + (num4 * num32)) * num27;
            result.M33 = ((num1 * num29) - (num2 * num31) + (num4 * num33)) * num27;
            result.M43 = -((num1 * num30) - (num2 * num32) + (num3 * num33)) * num27;
            Fix64 num34 = ((num7 * num12) - (num8 * num11));
            Fix64 num35 = ((num6 * num12) - (num8 * num10));
            Fix64 num36 = ((num6 * num11) - (num7 * num10));
            Fix64 num37 = ((num5 * num12) - (num8 * num9));
            Fix64 num38 = ((num5 * num11) - (num7 * num9));
            Fix64 num39 = ((num5 * num10) - (num6 * num9));
            result.M14 = -((num2 * num34) - (num3 * num35) + (num4 * num36)) * num27;
            result.M24 = ((num1 * num34) - (num3 * num37) + (num4 * num38)) * num27;
            result.M34 = -((num1 * num35) - (num2 * num37) + (num4 * num39)) * num27;
            result.M44 = ((num1 * num36) - (num2 * num38) + (num3 * num39)) * num27;


            /*


            ///
            // Use Laplace expansion theorem to calculate the inverse of a 4x4 matrix
            // 
            // 1. Calculate the 2x2 determinants needed the 4x4 determinant based on the 2x2 determinants 
            // 3. Create the adjugate matrix, which satisfies: A * adj(A) = det(A) * I
            // 4. Divide adjugate matrix with the determinant to find the inverse

            Fix64 det1, det2, det3, det4, det5, det6, det7, det8, det9, det10, det11, det12;
            Fix64 detMatrix;
            FindDeterminants(ref matrix, out detMatrix, out det1, out det2, out det3, out det4, out det5, out det6, 
                             out det7, out det8, out det9, out det10, out det11, out det12);

            Fix64 invDetMatrix = Fix64.One / detMatrix;

            Matrix ret; // Allow for matrix and result to point to the same structure

            ret.M11 = (matrix.M22*det12 - matrix.M23*det11 + matrix.M24*det10) * invDetMatrix;
            ret.M12 = (-matrix.M12*det12 + matrix.M13*det11 - matrix.M14*det10) * invDetMatrix;
            ret.M13 = (matrix.M42*det6 - matrix.M43*det5 + matrix.M44*det4) * invDetMatrix;
            ret.M14 = (-matrix.M32*det6 + matrix.M33*det5 - matrix.M34*det4) * invDetMatrix;
            ret.M21 = (-matrix.M21*det12 + matrix.M23*det9 - matrix.M24*det8) * invDetMatrix;
            ret.M22 = (matrix.M11*det12 - matrix.M13*det9 + matrix.M14*det8) * invDetMatrix;
            ret.M23 = (-matrix.M41*det6 + matrix.M43*det3 - matrix.M44*det2) * invDetMatrix;
            ret.M24 = (matrix.M31*det6 - matrix.M33*det3 + matrix.M34*det2) * invDetMatrix;
            ret.M31 = (matrix.M21*det11 - matrix.M22*det9 + matrix.M24*det7) * invDetMatrix;
            ret.M32 = (-matrix.M11*det11 + matrix.M12*det9 - matrix.M14*det7) * invDetMatrix;
            ret.M33 = (matrix.M41*det5 - matrix.M42*det3 + matrix.M44*det1) * invDetMatrix;
            ret.M34 = (-matrix.M31*det5 + matrix.M32*det3 - matrix.M34*det1) * invDetMatrix;
            ret.M41 = (-matrix.M21*det10 + matrix.M22*det8 - matrix.M23*det7) * invDetMatrix;
            ret.M42 = (matrix.M11*det10 - matrix.M12*det8 + matrix.M13*det7) * invDetMatrix;
            ret.M43 = (-matrix.M41*det4 + matrix.M42*det2 - matrix.M43*det1) * invDetMatrix;
            ret.M44 = (matrix.M31*det4 - matrix.M32*det2 + matrix.M33*det1) * invDetMatrix;

            result = ret;
            */
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="matrix1">The first <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and Fix64.One).</param>
        /// <returns>>The result of linear interpolation of the specified matrixes.</returns>
        public static FixMatrix Lerp(FixMatrix matrix1, FixMatrix matrix2, Fix64 amount)
        {
            matrix1.M11 += ((matrix2.M11 - matrix1.M11) * amount);
            matrix1.M12 += ((matrix2.M12 - matrix1.M12) * amount);
            matrix1.M13 += ((matrix2.M13 - matrix1.M13) * amount);
            matrix1.M14 += ((matrix2.M14 - matrix1.M14) * amount);
            matrix1.M21 += ((matrix2.M21 - matrix1.M21) * amount);
            matrix1.M22 += ((matrix2.M22 - matrix1.M22) * amount);
            matrix1.M23 += ((matrix2.M23 - matrix1.M23) * amount);
            matrix1.M24 += ((matrix2.M24 - matrix1.M24) * amount);
            matrix1.M31 += ((matrix2.M31 - matrix1.M31) * amount);
            matrix1.M32 += ((matrix2.M32 - matrix1.M32) * amount);
            matrix1.M33 += ((matrix2.M33 - matrix1.M33) * amount);
            matrix1.M34 += ((matrix2.M34 - matrix1.M34) * amount);
            matrix1.M41 += ((matrix2.M41 - matrix1.M41) * amount);
            matrix1.M42 += ((matrix2.M42 - matrix1.M42) * amount);
            matrix1.M43 += ((matrix2.M43 - matrix1.M43) * amount);
            matrix1.M44 += ((matrix2.M44 - matrix1.M44) * amount);
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains linear interpolation of the values in specified matrixes.
        /// </summary>
        /// <param name="matrix1">The first <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">The second <see cref="Vector2"/>.</param>
        /// <param name="amount">Weighting value(between 0.0 and Fix64.One).</param>
        /// <param name="result">The result of linear interpolation of the specified matrixes as an output parameter.</param>
        public static void Lerp(ref FixMatrix matrix1, ref FixMatrix matrix2, Fix64 amount, out FixMatrix result)
        {
            result.M11 = matrix1.M11 + ((matrix2.M11 - matrix1.M11) * amount);
            result.M12 = matrix1.M12 + ((matrix2.M12 - matrix1.M12) * amount);
            result.M13 = matrix1.M13 + ((matrix2.M13 - matrix1.M13) * amount);
            result.M14 = matrix1.M14 + ((matrix2.M14 - matrix1.M14) * amount);
            result.M21 = matrix1.M21 + ((matrix2.M21 - matrix1.M21) * amount);
            result.M22 = matrix1.M22 + ((matrix2.M22 - matrix1.M22) * amount);
            result.M23 = matrix1.M23 + ((matrix2.M23 - matrix1.M23) * amount);
            result.M24 = matrix1.M24 + ((matrix2.M24 - matrix1.M24) * amount);
            result.M31 = matrix1.M31 + ((matrix2.M31 - matrix1.M31) * amount);
            result.M32 = matrix1.M32 + ((matrix2.M32 - matrix1.M32) * amount);
            result.M33 = matrix1.M33 + ((matrix2.M33 - matrix1.M33) * amount);
            result.M34 = matrix1.M34 + ((matrix2.M34 - matrix1.M34) * amount);
            result.M41 = matrix1.M41 + ((matrix2.M41 - matrix1.M41) * amount);
            result.M42 = matrix1.M42 + ((matrix2.M42 - matrix1.M42) * amount);
            result.M43 = matrix1.M43 + ((matrix2.M43 - matrix1.M43) * amount);
            result.M44 = matrix1.M44 + ((matrix2.M44 - matrix1.M44) * amount);
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/>.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        public static FixMatrix Multiply(FixMatrix matrix1, FixMatrix matrix2)
        {
            Fix64 m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
            Fix64 m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
            Fix64 m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
            Fix64 m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
            Fix64 m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
            Fix64 m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
            Fix64 m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
            Fix64 m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
            Fix64 m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
            Fix64 m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
            Fix64 m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
            Fix64 m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
            Fix64 m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
            Fix64 m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
            Fix64 m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
            Fix64 m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
            matrix1.M12 = m12;
            matrix1.M13 = m13;
            matrix1.M14 = m14;
            matrix1.M21 = m21;
            matrix1.M22 = m22;
            matrix1.M23 = m23;
            matrix1.M24 = m24;
            matrix1.M31 = m31;
            matrix1.M32 = m32;
            matrix1.M33 = m33;
            matrix1.M34 = m34;
            matrix1.M41 = m41;
            matrix1.M42 = m42;
            matrix1.M43 = m43;
            matrix1.M44 = m44;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains a multiplication of two matrix.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/>.</param>
        /// <param name="result">Result of the matrix multiplication as an output parameter.</param>
        public static void Multiply(ref FixMatrix matrix1, ref FixMatrix matrix2, out FixMatrix result)
        {
            Fix64 m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
            Fix64 m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
            Fix64 m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
            Fix64 m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
            Fix64 m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
            Fix64 m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
            Fix64 m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
            Fix64 m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
            Fix64 m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
            Fix64 m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
            Fix64 m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
            Fix64 m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
            Fix64 m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
            Fix64 m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
            Fix64 m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
            Fix64 m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
            result.M11 = m11;
            result.M12 = m12;
            result.M13 = m13;
            result.M14 = m14;
            result.M21 = m21;
            result.M22 = m22;
            result.M23 = m23;
            result.M24 = m24;
            result.M31 = m31;
            result.M32 = m32;
            result.M33 = m33;
            result.M34 = m34;
            result.M41 = m41;
            result.M42 = m42;
            result.M43 = m43;
            result.M44 = m44;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains a multiplication of <see cref="FixMatrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static FixMatrix Multiply(FixMatrix matrix1, Fix64 scaleFactor)
        {
            matrix1.M11 *= scaleFactor;
            matrix1.M12 *= scaleFactor;
            matrix1.M13 *= scaleFactor;
            matrix1.M14 *= scaleFactor;
            matrix1.M21 *= scaleFactor;
            matrix1.M22 *= scaleFactor;
            matrix1.M23 *= scaleFactor;
            matrix1.M24 *= scaleFactor;
            matrix1.M31 *= scaleFactor;
            matrix1.M32 *= scaleFactor;
            matrix1.M33 *= scaleFactor;
            matrix1.M34 *= scaleFactor;
            matrix1.M41 *= scaleFactor;
            matrix1.M42 *= scaleFactor;
            matrix1.M43 *= scaleFactor;
            matrix1.M44 *= scaleFactor;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains a multiplication of <see cref="FixMatrix"/> and a scalar.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/>.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="result">Result of the matrix multiplication with a scalar as an output parameter.</param>
        public static void Multiply(ref FixMatrix matrix1, Fix64 scaleFactor, out FixMatrix result)
        {
            result.M11 = matrix1.M11 * scaleFactor;
            result.M12 = matrix1.M12 * scaleFactor;
            result.M13 = matrix1.M13 * scaleFactor;
            result.M14 = matrix1.M14 * scaleFactor;
            result.M21 = matrix1.M21 * scaleFactor;
            result.M22 = matrix1.M22 * scaleFactor;
            result.M23 = matrix1.M23 * scaleFactor;
            result.M24 = matrix1.M24 * scaleFactor;
            result.M31 = matrix1.M31 * scaleFactor;
            result.M32 = matrix1.M32 * scaleFactor;
            result.M33 = matrix1.M33 * scaleFactor;
            result.M34 = matrix1.M34 * scaleFactor;
            result.M41 = matrix1.M41 * scaleFactor;
            result.M42 = matrix1.M42 * scaleFactor;
            result.M43 = matrix1.M43 * scaleFactor;
            result.M44 = matrix1.M44 * scaleFactor;

        }

        /// <summary>
        /// Copy the values of specified <see cref="FixMatrix"/> to the Fix64 array.
        /// </summary>
        /// <param name="matrix">The source <see cref="FixMatrix"/>.</param>
        /// <returns>The array which matrix values will be stored.</returns>
        /// <remarks>
        /// Required for OpenGL 2.0 projection matrix stuff.
        /// </remarks>
        public static Fix64[] ToFloatArray(FixMatrix matrix)
        {
            Fix64[] matarray = [
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            ];
            return matarray;
        }

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/>.</param>
        /// <returns>Result of the matrix negation.</returns>
        public static FixMatrix Negate(FixMatrix matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }

        /// <summary>
        /// Returns a matrix with the all values negated.
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/>.</param>
        /// <param name="result">Result of the matrix negation as an output parameter.</param>
        public static void Negate(ref FixMatrix matrix, out FixMatrix result)
        {
            result.M11 = -matrix.M11;
            result.M12 = -matrix.M12;
            result.M13 = -matrix.M13;
            result.M14 = -matrix.M14;
            result.M21 = -matrix.M21;
            result.M22 = -matrix.M22;
            result.M23 = -matrix.M23;
            result.M24 = -matrix.M24;
            result.M31 = -matrix.M31;
            result.M32 = -matrix.M32;
            result.M33 = -matrix.M33;
            result.M34 = -matrix.M34;
            result.M41 = -matrix.M41;
            result.M42 = -matrix.M42;
            result.M43 = -matrix.M43;
            result.M44 = -matrix.M44;
        }

        /// <summary>
        /// Adds two matrixes.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the add sign.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/> on the right of the add sign.</param>
        /// <returns>Sum of the matrixes.</returns>
        public static FixMatrix operator +(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 += matrix2.M11;
            matrix1.M12 += matrix2.M12;
            matrix1.M13 += matrix2.M13;
            matrix1.M14 += matrix2.M14;
            matrix1.M21 += matrix2.M21;
            matrix1.M22 += matrix2.M22;
            matrix1.M23 += matrix2.M23;
            matrix1.M24 += matrix2.M24;
            matrix1.M31 += matrix2.M31;
            matrix1.M32 += matrix2.M32;
            matrix1.M33 += matrix2.M33;
            matrix1.M34 += matrix2.M34;
            matrix1.M41 += matrix2.M41;
            matrix1.M42 += matrix2.M42;
            matrix1.M43 += matrix2.M43;
            matrix1.M44 += matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by the elements of another <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the div sign.</param>
        /// <param name="matrix2">Divisor <see cref="FixMatrix"/> on the right of the div sign.</param>
        /// <returns>The result of dividing the matrixes.</returns>
        public static FixMatrix operator /(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 /= matrix2.M11;
            matrix1.M12 /= matrix2.M12;
            matrix1.M13 /= matrix2.M13;
            matrix1.M14 /= matrix2.M14;
            matrix1.M21 /= matrix2.M21;
            matrix1.M22 /= matrix2.M22;
            matrix1.M23 /= matrix2.M23;
            matrix1.M24 /= matrix2.M24;
            matrix1.M31 /= matrix2.M31;
            matrix1.M32 /= matrix2.M32;
            matrix1.M33 /= matrix2.M33;
            matrix1.M34 /= matrix2.M34;
            matrix1.M41 /= matrix2.M41;
            matrix1.M42 /= matrix2.M42;
            matrix1.M43 /= matrix2.M43;
            matrix1.M44 /= matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Divides the elements of a <see cref="FixMatrix"/> by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/> on the left of the div sign.</param>
        /// <param name="divider">Divisor scalar on the right of the div sign.</param>
        /// <returns>The result of dividing a matrix by a scalar.</returns>
        public static FixMatrix operator /(FixMatrix matrix, Fix64 divider)
        {
            Fix64 num = Fix64.One / divider;
            matrix.M11 *= num;
            matrix.M12 *= num;
            matrix.M13 *= num;
            matrix.M14 *= num;
            matrix.M21 *= num;
            matrix.M22 *= num;
            matrix.M23 *= num;
            matrix.M24 *= num;
            matrix.M31 *= num;
            matrix.M32 *= num;
            matrix.M33 *= num;
            matrix.M34 *= num;
            matrix.M41 *= num;
            matrix.M42 *= num;
            matrix.M43 *= num;
            matrix.M44 *= num;
            return matrix;
        }

        /// <summary>
        /// Compares whether two <see cref="FixMatrix"/> instances are equal without any tolerance.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the equal sign.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/> on the right of the equal sign.</param>
        /// <returns><c>true</c> if the instances are equal; <c>false</c> otherwise.</returns>
        public static bool operator ==(FixMatrix matrix1, FixMatrix matrix2)
        {
            return (
                matrix1.M11 == matrix2.M11 &&
                matrix1.M12 == matrix2.M12 &&
                matrix1.M13 == matrix2.M13 &&
                matrix1.M14 == matrix2.M14 &&
                matrix1.M21 == matrix2.M21 &&
                matrix1.M22 == matrix2.M22 &&
                matrix1.M23 == matrix2.M23 &&
                matrix1.M24 == matrix2.M24 &&
                matrix1.M31 == matrix2.M31 &&
                matrix1.M32 == matrix2.M32 &&
                matrix1.M33 == matrix2.M33 &&
                matrix1.M34 == matrix2.M34 &&
                matrix1.M41 == matrix2.M41 &&
                matrix1.M42 == matrix2.M42 &&
                matrix1.M43 == matrix2.M43 &&
                matrix1.M44 == matrix2.M44
            );
        }

        /// <summary>
        /// Compares whether two <see cref="FixMatrix"/> instances are not equal without any tolerance.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the not equal sign.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/> on the right of the not equal sign.</param>
        /// <returns><c>true</c> if the instances are not equal; <c>false</c> otherwise.</returns>
        public static bool operator !=(FixMatrix matrix1, FixMatrix matrix2)
        {
            return (
                matrix1.M11 != matrix2.M11 ||
                matrix1.M12 != matrix2.M12 ||
                matrix1.M13 != matrix2.M13 ||
                matrix1.M14 != matrix2.M14 ||
                matrix1.M21 != matrix2.M21 ||
                matrix1.M22 != matrix2.M22 ||
                matrix1.M23 != matrix2.M23 ||
                matrix1.M24 != matrix2.M24 ||
                matrix1.M31 != matrix2.M31 ||
                matrix1.M32 != matrix2.M32 ||
                matrix1.M33 != matrix2.M33 ||
                matrix1.M34 != matrix2.M34 ||
                matrix1.M41 != matrix2.M41 ||
                matrix1.M42 != matrix2.M42 ||
                matrix1.M43 != matrix2.M43 ||
                matrix1.M44 != matrix2.M44
            );
        }

        /// <summary>
        /// Multiplies two matrixes.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the mul sign.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/> on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication.</returns>
        /// <remarks>
        /// Using matrix multiplication algorithm - see http://en.wikipedia.org/wiki/Matrix_multiplication.
        /// </remarks>
        public static FixMatrix operator *(FixMatrix matrix1, FixMatrix matrix2)
        {
            Fix64 m11 = (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21) + (matrix1.M13 * matrix2.M31) + (matrix1.M14 * matrix2.M41);
            Fix64 m12 = (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22) + (matrix1.M13 * matrix2.M32) + (matrix1.M14 * matrix2.M42);
            Fix64 m13 = (matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23) + (matrix1.M13 * matrix2.M33) + (matrix1.M14 * matrix2.M43);
            Fix64 m14 = (matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24) + (matrix1.M13 * matrix2.M34) + (matrix1.M14 * matrix2.M44);
            Fix64 m21 = (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21) + (matrix1.M23 * matrix2.M31) + (matrix1.M24 * matrix2.M41);
            Fix64 m22 = (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22) + (matrix1.M23 * matrix2.M32) + (matrix1.M24 * matrix2.M42);
            Fix64 m23 = (matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23) + (matrix1.M23 * matrix2.M33) + (matrix1.M24 * matrix2.M43);
            Fix64 m24 = (matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24) + (matrix1.M23 * matrix2.M34) + (matrix1.M24 * matrix2.M44);
            Fix64 m31 = (matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21) + (matrix1.M33 * matrix2.M31) + (matrix1.M34 * matrix2.M41);
            Fix64 m32 = (matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22) + (matrix1.M33 * matrix2.M32) + (matrix1.M34 * matrix2.M42);
            Fix64 m33 = (matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23) + (matrix1.M33 * matrix2.M33) + (matrix1.M34 * matrix2.M43);
            Fix64 m34 = (matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24) + (matrix1.M33 * matrix2.M34) + (matrix1.M34 * matrix2.M44);
            Fix64 m41 = (matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21) + (matrix1.M43 * matrix2.M31) + (matrix1.M44 * matrix2.M41);
            Fix64 m42 = (matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22) + (matrix1.M43 * matrix2.M32) + (matrix1.M44 * matrix2.M42);
            Fix64 m43 = (matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23) + (matrix1.M43 * matrix2.M33) + (matrix1.M44 * matrix2.M43);
            Fix64 m44 = (matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24) + (matrix1.M43 * matrix2.M34) + (matrix1.M44 * matrix2.M44);
            matrix1.M11 = m11;
            matrix1.M12 = m12;
            matrix1.M13 = m13;
            matrix1.M14 = m14;
            matrix1.M21 = m21;
            matrix1.M22 = m22;
            matrix1.M23 = m23;
            matrix1.M24 = m24;
            matrix1.M31 = m31;
            matrix1.M32 = m32;
            matrix1.M33 = m33;
            matrix1.M34 = m34;
            matrix1.M41 = m41;
            matrix1.M42 = m42;
            matrix1.M43 = m43;
            matrix1.M44 = m44;
            return matrix1;
        }

        /// <summary>
        /// Multiplies the elements of matrix by a scalar.
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/> on the left of the mul sign.</param>
        /// <param name="scaleFactor">Scalar value on the right of the mul sign.</param>
        /// <returns>Result of the matrix multiplication with a scalar.</returns>
        public static FixMatrix operator *(FixMatrix matrix, Fix64 scaleFactor)
        {
            matrix.M11 *= scaleFactor;
            matrix.M12 *= scaleFactor;
            matrix.M13 *= scaleFactor;
            matrix.M14 *= scaleFactor;
            matrix.M21 *= scaleFactor;
            matrix.M22 *= scaleFactor;
            matrix.M23 *= scaleFactor;
            matrix.M24 *= scaleFactor;
            matrix.M31 *= scaleFactor;
            matrix.M32 *= scaleFactor;
            matrix.M33 *= scaleFactor;
            matrix.M34 *= scaleFactor;
            matrix.M41 *= scaleFactor;
            matrix.M42 *= scaleFactor;
            matrix.M43 *= scaleFactor;
            matrix.M44 *= scaleFactor;
            return matrix;
        }

        /// <summary>
        /// Subtracts the values of one <see cref="FixMatrix"/> from another <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="matrix1">Source <see cref="FixMatrix"/> on the left of the sub sign.</param>
        /// <param name="matrix2">Source <see cref="FixMatrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the matrix subtraction.</returns>
        public static FixMatrix operator -(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 -= matrix2.M11;
            matrix1.M12 -= matrix2.M12;
            matrix1.M13 -= matrix2.M13;
            matrix1.M14 -= matrix2.M14;
            matrix1.M21 -= matrix2.M21;
            matrix1.M22 -= matrix2.M22;
            matrix1.M23 -= matrix2.M23;
            matrix1.M24 -= matrix2.M24;
            matrix1.M31 -= matrix2.M31;
            matrix1.M32 -= matrix2.M32;
            matrix1.M33 -= matrix2.M33;
            matrix1.M34 -= matrix2.M34;
            matrix1.M41 -= matrix2.M41;
            matrix1.M42 -= matrix2.M42;
            matrix1.M43 -= matrix2.M43;
            matrix1.M44 -= matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Inverts values in the specified <see cref="FixMatrix"/>.
        /// </summary>
        /// <param name="matrix">Source <see cref="FixMatrix"/> on the right of the sub sign.</param>
        /// <returns>Result of the inversion.</returns>
        public static FixMatrix operator -(FixMatrix matrix)
        {
            matrix.M11 = -matrix.M11;
            matrix.M12 = -matrix.M12;
            matrix.M13 = -matrix.M13;
            matrix.M14 = -matrix.M14;
            matrix.M21 = -matrix.M21;
            matrix.M22 = -matrix.M22;
            matrix.M23 = -matrix.M23;
            matrix.M24 = -matrix.M24;
            matrix.M31 = -matrix.M31;
            matrix.M32 = -matrix.M32;
            matrix.M33 = -matrix.M33;
            matrix.M34 = -matrix.M34;
            matrix.M41 = -matrix.M41;
            matrix.M42 = -matrix.M42;
            matrix.M43 = -matrix.M43;
            matrix.M44 = -matrix.M44;
            return matrix;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="matrix1">The first <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">The second <see cref="FixMatrix"/>.</param>
        /// <returns>The result of the matrix subtraction.</returns>
        public static FixMatrix Subtract(FixMatrix matrix1, FixMatrix matrix2)
        {
            matrix1.M11 -= matrix2.M11;
            matrix1.M12 -= matrix2.M12;
            matrix1.M13 -= matrix2.M13;
            matrix1.M14 -= matrix2.M14;
            matrix1.M21 -= matrix2.M21;
            matrix1.M22 -= matrix2.M22;
            matrix1.M23 -= matrix2.M23;
            matrix1.M24 -= matrix2.M24;
            matrix1.M31 -= matrix2.M31;
            matrix1.M32 -= matrix2.M32;
            matrix1.M33 -= matrix2.M33;
            matrix1.M34 -= matrix2.M34;
            matrix1.M41 -= matrix2.M41;
            matrix1.M42 -= matrix2.M42;
            matrix1.M43 -= matrix2.M43;
            matrix1.M44 -= matrix2.M44;
            return matrix1;
        }

        /// <summary>
        /// Creates a new <see cref="FixMatrix"/> that contains subtraction of one matrix from another.
        /// </summary>
        /// <param name="matrix1">The first <see cref="FixMatrix"/>.</param>
        /// <param name="matrix2">The second <see cref="FixMatrix"/>.</param>
        /// <param name="result">The result of the matrix subtraction as an output parameter.</param>
        public static void Subtract(ref FixMatrix matrix1, ref FixMatrix matrix2, out FixMatrix result)
        {
            result.M11 = matrix1.M11 - matrix2.M11;
            result.M12 = matrix1.M12 - matrix2.M12;
            result.M13 = matrix1.M13 - matrix2.M13;
            result.M14 = matrix1.M14 - matrix2.M14;
            result.M21 = matrix1.M21 - matrix2.M21;
            result.M22 = matrix1.M22 - matrix2.M22;
            result.M23 = matrix1.M23 - matrix2.M23;
            result.M24 = matrix1.M24 - matrix2.M24;
            result.M31 = matrix1.M31 - matrix2.M31;
            result.M32 = matrix1.M32 - matrix2.M32;
            result.M33 = matrix1.M33 - matrix2.M33;
            result.M34 = matrix1.M34 - matrix2.M34;
            result.M41 = matrix1.M41 - matrix2.M41;
            result.M42 = matrix1.M42 - matrix2.M42;
            result.M43 = matrix1.M43 - matrix2.M43;
            result.M44 = matrix1.M44 - matrix2.M44;
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="FixMatrix"/> in the format:
        /// {M11:[<see cref="M11"/>] M12:[<see cref="M12"/>] M13:[<see cref="M13"/>] M14:[<see cref="M14"/>]}
        /// {M21:[<see cref="M21"/>] M12:[<see cref="M22"/>] M13:[<see cref="M23"/>] M14:[<see cref="M24"/>]}
        /// {M31:[<see cref="M31"/>] M32:[<see cref="M32"/>] M33:[<see cref="M33"/>] M34:[<see cref="M34"/>]}
        /// {M41:[<see cref="M41"/>] M42:[<see cref="M42"/>] M43:[<see cref="M43"/>] M44:[<see cref="M44"/>]}
        /// </summary>
        /// <returns>A <see cref="string"/> representation of this <see cref="FixMatrix"/>.</returns>
        public override readonly string ToString()
        {
            return "{M11:" + this.M11 + " M12:" + this.M12 + " M13:" + this.M13 + " M14:" + this.M14 + "}"
                + " {M21:" + this.M21 + " M22:" + this.M22 + " M23:" + this.M23 + " M24:" + this.M24 + "}"
                + " {M31:" + this.M31 + " M32:" + this.M32 + " M33:" + this.M33 + " M34:" + this.M34 + "}"
                + " {M41:" + this.M41 + " M42:" + this.M42 + " M43:" + this.M43 + " M44:" + this.M44 + "}";
        }

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <returns>The new <see cref="FixMatrix"/> which contains the transposing result.</returns>
        public static FixMatrix Transpose(FixMatrix matrix)
        {
            FixMatrix.Transpose(ref matrix, out FixMatrix ret);
            return ret;
        }

        /// <summary>
        /// Swap the matrix rows and columns.
        /// </summary>
        /// <param name="matrix">The matrix for transposing operation.</param>
        /// <param name="result">The new <see cref="FixMatrix"/> which contains the transposing result as an output parameter.</param>
        public static void Transpose(ref FixMatrix matrix, out FixMatrix result)
        {
            FixMatrix ret;

            ret.M11 = matrix.M11;
            ret.M12 = matrix.M21;
            ret.M13 = matrix.M31;
            ret.M14 = matrix.M41;

            ret.M21 = matrix.M12;
            ret.M22 = matrix.M22;
            ret.M23 = matrix.M32;
            ret.M24 = matrix.M42;

            ret.M31 = matrix.M13;
            ret.M32 = matrix.M23;
            ret.M33 = matrix.M33;
            ret.M34 = matrix.M43;

            ret.M41 = matrix.M14;
            ret.M42 = matrix.M24;
            ret.M43 = matrix.M34;
            ret.M44 = matrix.M44;

            result = ret;
        }

        public readonly void Deconstruct(out Fix64 x, out Fix64 y, out Fix64 cos, out Fix64 sin)
        {
            x = this.M41;
            y = this.M42;
            cos = this.M11;
            sin = this.M12;
        }

        public readonly FixTransform2D ToFixTransform2D()
        {
            return new FixTransform2D(
                x: this.M41,
                y: this.M42,
                cos: this.M11,
                sin: this.M12);
        }
        #endregion
    }
}