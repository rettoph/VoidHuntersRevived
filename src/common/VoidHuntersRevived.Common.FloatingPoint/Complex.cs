using System.Runtime.InteropServices;

namespace VoidHuntersRevived.Common.FloatingPoint
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct Complex(float real, float imaginary)
    {
        public static readonly Complex One = new(1f, 0f);
        public static readonly Complex ImaginaryOne = new(0f, 1f);

        public Complex(float phase) : this(MathF.Cos(phase), MathF.Sin(phase))
        {

        }

        /// <summary>
        /// The real value. Represents x rotation, or cos(Phase)
        /// </summary>
        [FieldOffset(0)]
        public float Real = real;

        /// <summary>
        /// The imaginary value. Represents y rotation, or sin(Phase)
        /// </summary>
        [FieldOffset(4)]
        public float Imaginary = imaginary;

        public float Phase
        {
            readonly get { return float.Atan2(this.Imaginary, this.Real); }
            set
            {
                if (value == 0f)
                {
                    this = Complex.One;
                    return;
                }

                this.Real = float.Cos(value);
                this.Imaginary = float.Sin(value);
            }
        }

        public override readonly string ToString()
        {
            return $"{{ Real = {this.Real}, Imaginary = {this.Imaginary} }}";
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Complex complex && complex == this;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Imaginary, this.Real);
        }

        public static bool operator ==(Complex left, Complex right)
        {
            return left.Real == right.Real &&
                left.Imaginary == right.Imaginary;
        }

        public static bool operator !=(Complex left, Complex right)
        {
            return left.Real != right.Real ||
                left.Imaginary != right.Imaginary;
        }
    }
}
