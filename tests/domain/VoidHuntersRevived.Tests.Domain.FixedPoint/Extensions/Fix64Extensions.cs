using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions
{
    public static class Fix64Extensions
    {
        public static void AssertWithinEpsilon(this Fix64 left, Fix64 right, Fix64 epsilon)
        {
            Fix64 diff = Fix64.Abs(left - right);
            Assert.True(diff < epsilon);
        }
    }
}
