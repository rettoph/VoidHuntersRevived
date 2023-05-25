using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FixedMath64 = FixedMath.NET.Fix64;

namespace VoidHuntersRevived.Common
{
    public struct Fix64
    {
        readonly long m_rawValue;

        // Precision of this type is 2^-32, that is 2,3283064365386962890625E-10
        public static readonly decimal Precision = FixedMath64.Precision;
        public static readonly Fix64 MaxValue = FixedMath64.MaxValue;
        public static readonly Fix64 MinValue = FixedMath64.MinValue;
        public static readonly Fix64 One = FixedMath64.One;
        public static readonly Fix64 Zero = FixedMath64.Zero;
        /// <summary>
        /// The value of Pi
        /// </summary>
        public static readonly Fix64 Pi = FixedMath64.Pi;
        public static readonly Fix64 PiOver2 = FixedMath64.PiOver2;
        public static readonly Fix64 PiTimes2 = FixedMath64.PiTimes2;
        public static readonly Fix64 PiInv = FixedMath64.PiInv;
        public static readonly Fix64 PiOver2Inv = FixedMath64.PiOver2Inv;

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(Fix64 value)
        {
            return FixedMath64.Sign(value);
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static Fix64 Abs(Fix64 value)
        {
            return FixedMath64.Abs(value);
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// </summary>
        public static Fix64 FastAbs(Fix64 value)
        {
            return FixedMath64.FastAbs(value);
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static Fix64 Floor(Fix64 value)
        {
            return FixedMath64.Floor(value);
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static Fix64 Ceiling(Fix64 value)
        {
            return FixedMath64.Ceiling(value);
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static Fix64 Round(Fix64 value)
        {
            return FixedMath64.Round(value);
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static Fix64 operator +(Fix64 x, Fix64 y)
        {
            return (FixedMath64)x + (FixedMath64)y;
        }

        /// <summary>
        /// Adds x and y witout performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static Fix64 FastAdd(Fix64 x, Fix64 y)
        {
            return new Fix64(x.m_rawValue + y.m_rawValue);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static Fix64 operator -(Fix64 x, Fix64 y)
        {
            return (FixedMath64)x - (FixedMath64)y;
        }

        /// <summary>
        /// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static Fix64 FastSub(Fix64 x, Fix64 y)
        {
            return new Fix64(x.m_rawValue - y.m_rawValue);
        }

        public static Fix64 operator *(Fix64 x, Fix64 y)
        {
            return (FixedMath64)x * (FixedMath64)y;
        }

        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static Fix64 FastMul(Fix64 x, Fix64 y)
        {

            return FixedMath64.FastMul(x, y);
        }

        public static Fix64 operator /(Fix64 x, Fix64 y)
        {
            return (FixedMath64)x / (FixedMath64)y;
        }

        public static Fix64 operator %(Fix64 x, Fix64 y)
        {
            return (FixedMath64)x % (FixedMath64)y;
        }

        /// <summary>
        /// Performs modulo as fast as possible; throws if x == MinValue and y == -1.
        /// Use the operator (%) for a more reliable but slower modulo.
        /// </summary>
        public static Fix64 FastMod(Fix64 x, Fix64 y)
        {
            return FixedMath64.FastMod(x, y);
        }

        public static Fix64 operator -(Fix64 x)
        {
            return -(FixedMath64)x;
        }

        public static bool operator ==(Fix64 x, Fix64 y)
        {
            return x.m_rawValue == y.m_rawValue;
        }

        public static bool operator !=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue != y.m_rawValue;
        }

        public static bool operator >(Fix64 x, Fix64 y)
        {
            return x.m_rawValue > y.m_rawValue;
        }

        public static bool operator <(Fix64 x, Fix64 y)
        {
            return x.m_rawValue < y.m_rawValue;
        }

        public static bool operator >=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue >= y.m_rawValue;
        }

        public static bool operator <=(Fix64 x, Fix64 y)
        {
            return x.m_rawValue <= y.m_rawValue;
        }

        /// <summary>
        /// Returns the natural logarithm of a specified number.
        /// Provides at least 7 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static Fix64 Ln(Fix64 x)
        {
            return FixedMath64.Ln(x);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// Provides about 5 digits of accuracy for the result.
        /// </summary>
        /// <exception cref="DivideByZeroException">
        /// The base was zero, with a negative exponent
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The base was negative, with a non-zero exponent
        /// </exception>
        public static Fix64 Pow(Fix64 b, Fix64 exp)
        {
            return FixedMath64.Pow(b, exp);
        }

        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static Fix64 Sqrt(Fix64 x)
        {
            return FixedMath64.Sqrt(x);
        }

        /// <summary>
        /// Returns the Sine of x.
        /// The relative error is less than 1E-10 for x in [-2PI, 2PI], and less than 1E-7 in the worst case.
        /// </summary>
        public static Fix64 Sin(Fix64 x)
        {
            return FixedMath64.Sin(x);
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static Fix64 FastSin(Fix64 x)
        {
            return FixedMath64.FastSin(x);
        }

        /// <summary>
        /// Returns the cosine of x.
        /// The relative error is less than 1E-10 for x in [-2PI, 2PI], and less than 1E-7 in the worst case.
        /// </summary>
        public static Fix64 Cos(Fix64 x)
        {
            return FixedMath64.Cos(x);
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static Fix64 FastCos(Fix64 x)
        {
            return FixedMath64.FastCos(x);
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static Fix64 Tan(Fix64 x)
        {
            return FixedMath64.Tan(x);
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static Fix64 Acos(Fix64 x)
        {
            return FixedMath64.Acos(x);
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static Fix64 Atan(Fix64 z)
        {
            return FixedMath64.Atan(z);
        }

        public static Fix64 Atan2(Fix64 y, Fix64 x)
        {
            return FixedMath64.Atan2(y, x);
        }



        public static explicit operator Fix64(long value)
        {
            return (FixedMath64)value;
        }
        public static explicit operator long(Fix64 value)
        {
            return (long)(FixedMath64)value;
        }
        public static explicit operator Fix64(float value)
        {
            return (FixedMath64)value;
        }
        public static explicit operator float(Fix64 value)
        {
            return (float)(FixedMath64)value;
        }
        public static explicit operator Fix64(double value)
        {
            return (FixedMath64)value;
        }
        public static explicit operator double(Fix64 value)
        {
            return (double)(FixedMath64)(value);
        }
        public static explicit operator Fix64(decimal value)
        {
            return (FixedMath64)value;
        }
        public static explicit operator decimal(Fix64 value)
        {
            return (decimal)(FixedMath64)value;
        }

        public override bool Equals(object? obj)
        {
            if(obj is null)
            {
                return false;
            }

            return ((FixedMath64)obj).Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_rawValue.GetHashCode();
        }

        public bool Equals(Fix64 other)
        {
            return m_rawValue == other.m_rawValue;
        }

        public int CompareTo(Fix64 other)
        {
            return m_rawValue.CompareTo(other.m_rawValue);
        }

        public override string ToString()
        {
            // Up to 10 decimal places
            return ((decimal)this).ToString("0.##########");
        }

        public static Fix64 FromRaw(long rawValue)
        {
            return new Fix64(rawValue);
        }

        // turn into a Console Application and use this to generate the look-up tables
        //static void Main(string[] args)
        //{
        //    GenerateSinLut();
        //    GenerateTanLut();
        //}

        /// <summary>
        /// The underlying integer representation
        /// </summary>
        public long RawValue => m_rawValue;

        /// <summary>
        /// This is the constructor from raw value; it can only be used interally.
        /// </summary>
        /// <param name="rawValue"></param>
        Fix64(long rawValue)
        {
            m_rawValue = rawValue;
        }

        public static Fix64 Min(Fix64 value1, Fix64 value2)
        {
            if (value2 < value1)
            {
                return value2;
            }

            return value1;
        }

        public static Fix64 Max(Fix64 value1, Fix64 value2)
        {
            if (value2 > value1)
            {
                return value2;
            }

            return value1;
        }

        public static implicit operator Fix64(FixedMath64 fixedMath64)
        {
            return Unsafe.As<FixedMath64, Fix64>(ref fixedMath64);
        }

        public static implicit operator FixedMath64(Fix64 fix64)
        {
            return Unsafe.As<Fix64, FixedMath64>(ref fix64);
        }

        public static Fix64 Lerp(Fix64 v1, Fix64 v2, Fix64 amount)
        {
            return v1 + (v2 - v1) * amount;
        }
    }
}
