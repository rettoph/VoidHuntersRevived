using Autofac;
using Guppy.Core.Network.Common.Enums;
using Moq;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;
using VoidHuntersRevived.Tests.Common;

namespace VoidHuntersRevived.Tests.Domain.Simulations.Common.Extensions
{
    public static class AutoMockBuilderExtensions
    {
        private static StrategyTypeEnum[] PredictiveLockstep = [StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep];
        private static StrategyTypeEnum[] Lockstep = [StrategyTypeEnum.Lockstep];

        public static AutoMockBuilder RegisterSimulation(this AutoMockBuilder builder, VhId id, PeerType peerType, Dictionary<StrategyTypeEnum, IEngine[]> strategies)
        {
            return builder.ConfigureAllGuppyConfigurationAttributesInAssembly(typeof(Simulation).Assembly).Register(x => x.Register<ISimulation>(ctx =>
            {
                IMock<IStrategiesFactory> strategiesFactoryMock = MockBuilder<IStrategiesFactory>.Create()
                    .Setup<IEnumerable<IStrategy>, ISimulation, StrategyTypeEnum[]>(
                        factory => factory.BuildStrategies(It.IsAny<ISimulation>(), PredictiveLockstep),
                        (simulation, _) =>
                        {
                            List<IStrategy> instances = new List<IStrategy>();

                            if (peerType == PeerType.Client && strategies.ContainsKey(StrategyTypeEnum.Predictive))
                            {
                                instances.Add(ctx.Resolve<PredictiveStrategy>());
                            }
                            if (peerType == PeerType.Client && strategies.ContainsKey(StrategyTypeEnum.Lockstep))
                            {
                                instances.Add(ctx.Resolve<LockstepStrategy_Client>());
                            }
                            if (peerType == PeerType.Server && strategies.ContainsKey(StrategyTypeEnum.Lockstep))
                            {
                                instances.Add(ctx.Resolve<LockstepStrategy_Server>());
                            }

                            return instances;
                        })
                    .Build();

                return new Simulation(id, strategiesFactoryMock.Object, strategies.Keys.ToArray());

            }).AsSelf().AsImplementedInterfaces());
        }
    }
}
