using Guppy.Core.Network.Common.Enums;
using Moq;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common
{
    public class SimulationFactory
    {
        private static StrategyTypeEnum[] PredictiveLockstep = [StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep];
        private static StrategyTypeEnum[] Lockstep = [StrategyTypeEnum.Lockstep];


        public static Simulation Build(VhId id, PeerType peerType, Dictionary<StrategyTypeEnum, IEngine[]> strategies)
        {
            IMock<IStrategiesFactory> StrategiesFactoryMock = MockBuilder<IStrategiesFactory>.Create()
                .Setup<IEnumerable<IStrategy>, ISimulation, StrategyTypeEnum[]>(
                    factory => factory.BuildStrategies(It.IsAny<ISimulation>(), PredictiveLockstep),
                    (simulation, _) =>
                    {
                        return Enumerable.Empty<IStrategy>();
                    })
                .Build();

            return new Simulation(id, StrategiesFactoryMock.Object, strategies.Keys.ToArray());
        }
    }
}
