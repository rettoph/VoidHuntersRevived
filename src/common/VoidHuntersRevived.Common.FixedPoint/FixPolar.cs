namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixPolar(Fix64 length, Fix64 radians)
    {
        public Fix64 Length = length;
        public Fix64 Radians = radians;

        public readonly FixVector2 ToVector2() => new(
                x: Fix64.Cos(this.Radians) * this.Length,
                y: Fix64.Sin(this.Radians) * this.Length);

        public static FixPolar Rotate(FixPolar polar, Fix64 radians) => new(polar.Length, polar.Radians + radians);

        public static FixPolar operator *(FixPolar polar, Fix64 ratio)
        {
            return new FixPolar(polar.Length * ratio, polar.Radians);
        }
    }
}