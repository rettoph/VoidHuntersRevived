using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Tests.Domain.FixedPoint.Extensions;

namespace VoidHuntersRevived.Tests.Domain.FixedPoint
{
    public class FixComplexTests
    {
        public static readonly Fix64 RadianEpsilon = (Fix64)0.01f;

        [Fact]
        public void FixComplex_Radian_Test()
        {
            for (Fix64 i = -Fix64.PiTimes2; i < Fix64.PiTimes2; i += RadianEpsilon)
            {
                Fix64 wrapped = Fix64.WrapAngle(i);
                FixComplex complex = new(wrapped);

                wrapped.AssertWithinEpsilon(complex.Phase, RadianEpsilon);
            }
        }
    }
}
