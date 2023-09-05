using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.FixedPoint.Utilities
{
    public static class FixMatrixHelper
    {

        public static FixMatrix FastMultiplyTransformations(FixMatrix transformation1, FixMatrix transformation2)
        {
            Fix64 cos1cos2 = transformation1.M11 * transformation2.M11;
            Fix64 sin1sin2 = transformation1.M12 * transformation2.M12;
            Fix64 cos1sin2 = transformation1.M11 * transformation2.M12;
            Fix64 sin1cos2 = transformation1.M12 * transformation2.M11;
            Fix64 x1 = transformation1.M41;
            Fix64 x2 = transformation2.M41;
            Fix64 y1 = transformation1.M42;
            Fix64 y2 = transformation2.M42;
            Fix64 cos2 = transformation2.M11;
            Fix64 sin2 = transformation2.M12;

            FixMatrix result = new FixMatrix(
                cos1cos2 - sin1sin2,    cos1sin2 + sin1cos2,    Fix64.Zero, Fix64.Zero,
                -sin1cos2 - cos1sin2,   cos1cos2 - sin1sin2,    Fix64.Zero, Fix64.Zero,
                Fix64.Zero,             Fix64.Zero,             Fix64.One,  Fix64.Zero,
                (x1 * cos2) - (y1 * sin2) + x2, (x1 * sin2) + (y1 * cos2) + y2, Fix64.Zero, Fix64.One
            );

            return result;
        }
    }
}
