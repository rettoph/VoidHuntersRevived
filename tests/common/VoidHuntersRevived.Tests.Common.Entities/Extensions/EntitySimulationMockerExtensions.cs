using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Common.Entities.Interfaces;
using Xunit;

namespace VoidHuntersRevived.Tests.Common.Entities.Extensions
{
    public static class EntitySimulationMockerExtensions
    {
        public static Dictionary<StrategyTypeEnum, int> CalculateTotalEntities<TComponent>(
            this IEntitySimulationMocker simulation)
                where TComponent : unmanaged, IEntityComponent
        {
            return new Dictionary<StrategyTypeEnum, int>()
            {
                { StrategyTypeEnum.Predictive, simulation.EntityPredictiveStrategyMocker.EntityQueryServiceBuilder.Object.CalculateTotal<TComponent>() },
                { StrategyTypeEnum.Lockstep, simulation.EntityLockstepStrategyMocker.EntityQueryServiceBuilder.Object.CalculateTotal<TComponent>() },
            };
        }

        public static IEntitySimulationMocker AssertTotalEntities<TComponent>(
            this IEntitySimulationMocker simulation,
            int expected)
                where TComponent : unmanaged, IEntityComponent
        {
            Dictionary<StrategyTypeEnum, int> totals = simulation.CalculateTotalEntities<TComponent>();

            Assert.Equal(expected, totals[StrategyTypeEnum.Lockstep]);
            Assert.Equal(expected, totals[StrategyTypeEnum.Predictive]);

            return simulation;
        }

        public static IEntitySimulationMocker AssertTotalEntities<TComponent>(
            this IEntitySimulationMocker simulation,
            int lockstepExpected,
            int predictiveExpceted)
                where TComponent : unmanaged, IEntityComponent
        {
            Dictionary<StrategyTypeEnum, int> totals = simulation.CalculateTotalEntities<TComponent>();

            Assert.Equal(lockstepExpected, totals[StrategyTypeEnum.Lockstep]);
            Assert.Equal(predictiveExpceted, totals[StrategyTypeEnum.Predictive]);

            return simulation;
        }
    }
}
