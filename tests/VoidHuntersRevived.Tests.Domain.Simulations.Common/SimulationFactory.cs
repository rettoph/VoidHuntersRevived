using Autofac;
using Guppy.Core.Network.Common.Enums;
using Guppy.Tests.Common;
using Moq;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common
{
    public static class SimulationFactory
    {
        private static StrategyTypeEnum[] PredictiveLockstep = [StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep];
        private static StrategyTypeEnum[] Lockstep = [StrategyTypeEnum.Lockstep];


        public static Simulation Build(VhId id, PeerType peerType, StrategyTypeEnum[] strategies, Dictionary<StrategyTypeEnum, Func<ILifetimeScope, IEnumerable<IEngine>>>? customEnginesFactories, TickBuffer? tickBuffer)
        {
            IMock<IStrategiesFactory> strategiesFactoryMock = MockBuilder<IStrategiesFactory>.Create()
                .Setup<IEnumerable<IStrategy>, ISimulation, StrategyTypeEnum[]>(
                    factory => factory.BuildStrategies(It.IsAny<ISimulation>(), PredictiveLockstep),
                    (simulation, _) =>
                    {
                        List<IStrategy> instances = new List<IStrategy>();

                        if (peerType == PeerType.Client && strategies.Contains(StrategyTypeEnum.Predictive))
                        {
                            Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory = null;
                            customEnginesFactories?.TryGetValue(StrategyTypeEnum.Predictive, out customEnginesFactory);

                            instances.Add(StrategyFactory.BuildPredictive(simulation, customEnginesFactory));
                        }
                        if (peerType == PeerType.Client && strategies.Contains(StrategyTypeEnum.Lockstep))
                        {
                            Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory = null;
                            customEnginesFactories?.TryGetValue(StrategyTypeEnum.Predictive, out customEnginesFactory);

                            instances.Add(StrategyFactory.BuildClientLockstep(simulation, customEnginesFactory, tickBuffer));
                        }
                        if (peerType == PeerType.Server && strategies.Contains(StrategyTypeEnum.Lockstep))
                        {
                            Func<ILifetimeScope, IEnumerable<IEngine>>? customEnginesFactory = null;
                            customEnginesFactories?.TryGetValue(StrategyTypeEnum.Predictive, out customEnginesFactory);

                            instances.Add(StrategyFactory.BuildServerLockstep(simulation, customEnginesFactory));
                        }

                        return instances;
                    })
                .Build();

            return new Simulation(id, strategiesFactoryMock.Object, strategies);
        }
    }
}
