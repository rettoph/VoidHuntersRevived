
namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixComplex(Fix64 real, Fix64 imaginary)
    {
        public static readonly FixComplex One = new(Fix64.One, Fix64.Zero);
        public static readonly FixComplex ImaginaryOne = new(Fix64.Zero, Fix64.One);

        public FixComplex(Fix64 phase) : this(Fix64.Cos(phase), Fix64.Sin(phase))
        {

        }

        /// <summary>
        /// The real value. Represents x rotation, or cos(Phase)
        /// </summary>
        public Fix64 Real = real;

        /// <summary>
        /// The imaginary value. Represents y rotation, or sin(Phase)
        /// </summary>
        public Fix64 Imaginary = imaginary;

        public Fix64 Phase
        {
            readonly get { return Fix64.Atan2(this.Imaginary, this.Real); }
            set
            {
                if (value == Fix64.Zero)
                {
                    this = FixComplex.One;
                    return;
                }

                this.Real = Fix64.Cos(value);
                this.Imaginary = Fix64.Sin(value);
            }
        }

        public override readonly string ToString()
        {
            return $"{{ Real = {this.Real}, Imaginary = {this.Imaginary} }}";
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is FixComplex complex && complex == this;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(this.Imaginary, this.Real);
        }

        public static bool operator ==(FixComplex left, FixComplex right)
        {
            return left.Real == right.Real &&
                left.Imaginary == right.Imaginary;
        }

        public static bool operator !=(FixComplex left, FixComplex right)
        {
            return left.Real != right.Real ||
                left.Imaginary != right.Imaginary;
        }
    }
}