using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Common.Physics.Interfaces;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Entities.Extensions
{
    public static class PhysicsSimulationMockerExtensions
    {
        public static Dictionary<StrategyTypeEnum, int> CalculateTotalBodies(
            this IPhysicsSimulationMocker simulation)
        {
            return new Dictionary<StrategyTypeEnum, int>()
            {
                { StrategyTypeEnum.Predictive, simulation.PhysicsPredictiveStrategyMocker.SpaceBuilder.Object.BodyCount },
                { StrategyTypeEnum.Lockstep, simulation.PhysicsLockstepStrategyMocker.SpaceBuilder.Object.BodyCount },
            };
        }

        public static IPhysicsSimulationMocker AssertTotalBodies(
            this IPhysicsSimulationMocker simulation,
            int expected)
        {
            Dictionary<StrategyTypeEnum, int> totals = simulation.CalculateTotalBodies();

            Assert.Equal(expected, totals[StrategyTypeEnum.Lockstep]);
            Assert.Equal(expected, totals[StrategyTypeEnum.Predictive]);

            return simulation;
        }

        public static IPhysicsSimulationMocker AssertTotalEntities(
            this IPhysicsSimulationMocker simulation,
            int lockstepExpected,
            int predictiveExpceted)
        {
            Dictionary<StrategyTypeEnum, int> totals = simulation.CalculateTotalBodies();

            Assert.Equal(lockstepExpected, totals[StrategyTypeEnum.Lockstep]);
            Assert.Equal(predictiveExpceted, totals[StrategyTypeEnum.Predictive]);

            return simulation;
        }
    }
}
