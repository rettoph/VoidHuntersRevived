using Autofac;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Factories;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Domain.Simulations.Factories
{
    public sealed class StrategiesFactory : IStrategiesFactory
    {
        private readonly INetScope<IStrategy> _netScope;
        private readonly ISceneService _scenes;
        private readonly ITerminal _terminal;

        public StrategiesFactory(INetScope<IStrategy> netScope, ISceneService scenes, ITerminal terminal)
        {
            _netScope = netScope;
            _scenes = scenes;
            _terminal = terminal;
        }

        public IEnumerable<IStrategy> BuildStrategies(ISimulation simulation, StrategyTypeEnum[] strategies)
        {
            List<Type> strategyTypes = new List<Type>();
            if (_netScope.Group.Peer.Type == PeerType.Client && strategies.Contains(StrategyTypeEnum.Predictive))
            {
                strategyTypes.Add(typeof(PredictiveStrategy));
            }
            if (_netScope.Group.Peer.Type == PeerType.Client && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                strategyTypes.Add(typeof(LockstepStrategy_Client));
            }
            if (_netScope.Group.Peer.Type == PeerType.Server && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                strategyTypes.Add(typeof(LockstepStrategy_Server));
            }

            foreach (Type type in strategyTypes)
            {
                IStrategy strategy = (IStrategy)_scenes.Create(type, configuration =>
                {
                    configuration.WithContainerBuilder(builder =>
                    {
                        builder.RegisterInstance<ITerminal>(_terminal).AsImplementedInterfaces();

                        builder.RegisterInstance(simulation).As<ISimulation>();
                        builder.RegisterNetScope<IStrategy>(_netScope.Group.Peer.Type, _netScope.Group.Id);
                    });
                });

                yield return strategy;
            }
        }
    }
}
