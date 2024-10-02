namespace VoidHuntersRevived.Common.FixedPoint
{
    public struct FixPolar(Fix64 length, Fix64 radians)
    {
        public Fix64 Length = length;
        public Fix64 Radians = radians;

        public FixVector2 ToVector2()
        {
            return new FixVector2(
                x: Fix64.Cos(Radians) * Length,
                y: Fix64.Sin(Radians) * Length);
        }

        public static FixPolar Rotate(FixPolar polar, Fix64 radians)
        {
            return new FixPolar(polar.Length, polar.Radians + radians);
        }

        public static FixPolar operator *(FixPolar polar, Fix64 ratio)
        {
            return new FixPolar(polar.Length * ratio, polar.Radians);
        }
    }
}
