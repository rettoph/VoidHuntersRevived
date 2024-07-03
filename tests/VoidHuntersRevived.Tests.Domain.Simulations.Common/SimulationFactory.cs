using Moq;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common
{
    public class SimulationFactory
    {
        private static StrategyTypeEnum[] PredictiveLockstep = [StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep];
        private static StrategyTypeEnum[] Lockstep = [StrategyTypeEnum.Lockstep];

        private static Mock<IStrategiesFactory> StrategiesFactoryMock;

        static SimulationFactory()
        {
            StrategiesFactoryMock = new Mock<IStrategiesFactory>();

            // StrategiesFactoryMock.Setup(factory => factory.BuildStrategies(It.IsAny<ISimulation>(), PredictiveLockstep)).Returns()
        }

        public static Simulation Build(VhId id, params StrategyTypeEnum[] strategies)
        {
            return new Simulation(id, StrategiesFactoryMock.Object, strategies);
        }

        private static IEnumerable<IStrategy> BuildClientPredictiveLockstepStrategies()
        {
            throw new NotImplementedException();
        }
    }
}
