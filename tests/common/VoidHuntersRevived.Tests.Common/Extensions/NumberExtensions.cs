using System.Numerics;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Extensions
{
    public static class NumberExtensions
    {
        public static void AssertWithinEpsilon<T>(this T left, T right, T epsilon)
            where T : INumber<T>
        {
            T diff = T.Abs(left - right);
            Assert.True(diff < epsilon);
        }
    }
}
