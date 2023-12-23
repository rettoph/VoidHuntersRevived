using Microsoft.Xna.Framework;

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
                cos1cos2 - sin1sin2, cos1sin2 + sin1cos2, Fix64.Zero, Fix64.Zero,
                -sin1cos2 - cos1sin2, cos1cos2 - sin1sin2, Fix64.Zero, Fix64.Zero,
                Fix64.Zero, Fix64.Zero, Fix64.One, Fix64.Zero,
                (x1 * cos2) - (y1 * sin2) + x2, (x1 * sin2) + (y1 * cos2) + y2, Fix64.Zero, Fix64.One
            );

            return result;
        }

        public static Matrix FastMultiplyTransformationsToXnaMatrix(FixMatrix transformation1, FixMatrix transformation2)
        {
            float cos2 = (float)transformation2.M11;
            float sin2 = (float)transformation2.M12;
            float cos1cos2 = (float)(transformation1.M11) * cos2;
            float sin1sin2 = (float)(transformation1.M12) * sin2;
            float cos1sin2 = (float)(transformation1.M11) * sin2;
            float sin1cos2 = (float)(transformation1.M12) * cos2;
            float x1 = (float)transformation1.M41;
            float x2 = (float)transformation2.M41;
            float y1 = (float)transformation1.M42;
            float y2 = (float)transformation2.M42;

            Matrix result = new Matrix(
                cos1cos2 - sin1sin2, cos1sin2 + sin1cos2, 0, 0,
                -sin1cos2 - cos1sin2, cos1cos2 - sin1sin2, 0, 0,
                0, 0, 1, 0,
                (x1 * cos2) - (y1 * sin2) + x2, (x1 * sin2) + (y1 * cos2) + y2, 0, 1
            );

            return result;
        }
    }
}
