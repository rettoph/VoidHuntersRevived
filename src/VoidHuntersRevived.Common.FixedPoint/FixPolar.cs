using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common
{
    public struct FixPolar
    {
        public Fix64 Length;
        public Fix64 Radians;

        public FixPolar(Fix64 length, Fix64 radians)
        {
            Length = length;
            Radians = radians;
        }

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
    }
}
