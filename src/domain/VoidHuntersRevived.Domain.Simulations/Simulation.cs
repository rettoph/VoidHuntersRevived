using Autofac;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Extensions;
using Guppy.Game.Common.Services;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class Simulation : ISimulation
    {
        private Dictionary<StrategyTypeEnum, IStrategy> _strategyTypes;
        private List<IStrategy> _strategies;
        private ILifetimeScope _scope;

        public VhId Id { get; }

        public IStrategy this[StrategyTypeEnum type] => _strategyTypes[type];

        public IReadOnlyCollection<IStrategy> Strategies { get; }

        public Simulation(VhId id, ILifetimeScope scope, params StrategyTypeEnum[] strategies)
        {
            _scope = scope;
            _strategies = new List<IStrategy>();
            _strategyTypes = new Dictionary<StrategyTypeEnum, IStrategy>();

            this.Id = id;
            this.Strategies = new ReadOnlyCollection<IStrategy>(_strategies);

            INetScope<IStrategy> netScope = _scope.Resolve<INetScope<IStrategy>>();
            List<Type> simulationTypes = new List<Type>();
            if (netScope.Group.Peer.Type == PeerType.Client && strategies.Contains(StrategyTypeEnum.Predictive))
            {
                simulationTypes.Add(typeof(PredictiveStrategy));
            }
            if (netScope.Group.Peer.Type == PeerType.Client && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                simulationTypes.Add(typeof(LockstepStrategy_Client));
            }
            if (netScope.Group.Peer.Type == PeerType.Server && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                simulationTypes.Add(typeof(LockstepStrategy_Server));
            }

            ISceneService scenes = _scope.Resolve<ISceneService>();
            ITerminal terminal = _scope.Resolve<ITerminal>();
            foreach (Type simulationType in simulationTypes)
            {
                IStrategy strategy = (IStrategy)scenes.Create(simulationType, configuration =>
                {
                    configuration.WithContainerBuilder(builder =>
                    {
                        builder.RegisterInstanceFrom<ITerminal>(scope).AsImplementedInterfaces();

                        builder.RegisterInstance(this).As<ISimulation>();
                        builder.RegisterNetScope<IStrategy>(netScope.Group.Peer.Type, netScope.Group.Id);
                    });
                });

                _strategyTypes.Add(strategy.Type, strategy);
                _strategies.Add(strategy);
            }

            foreach (IStrategy strategy in _strategies)
            {
                strategy.Initialize(this);
            }
        }

        public void Dispose()
        {
            foreach (IStrategy strategy in _strategies)
            {
                strategy.Dispose();
            }
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = _strategies.Count - 1; i >= 0; i--)
            {
                _strategies[i].Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < _strategies.Count; i++)
            {
                _strategies[i].Draw(gameTime);
            }
        }

        public IStrategy? First(params StrategyTypeEnum[] strategies)
        {
            foreach (StrategyTypeEnum strategyType in strategies)
            {
                if (_strategyTypes.TryGetValue(strategyType, out IStrategy? strategy))
                {
                    return strategy;
                }
            }

            return null;
        }

        public void Input(VhId sourceId, IInputData data)
        {
            foreach (IStrategy strategy in _strategies)
            {
                strategy.Input(sourceId, data);
            }
        }
    }
}
