using System.Collections.ObjectModel;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService, IDisposable
    {
        private readonly INetScope<IStrategy> _netScope;
        private readonly ISceneService _scenes;
        private readonly List<ISimulation> _simulations;

        public ReadOnlyCollection<ISimulation> Instances { get; }

        public SimulationService(INetScope<IStrategy> netScope, ISceneService scenes)
        {
            this._netScope = netScope;
            this._scenes = scenes;
            this._simulations = [];

            this.Instances = new ReadOnlyCollection<ISimulation>(this._simulations);
        }

        public void Dispose()
        {
            foreach (ISimulation simulation in this._simulations)
            {
                simulation.Dispose();
            }
        }

        public ISimulation Create(VhId id, params StrategyTypeEnum[] strategies)
        {
            ISimulation simulation = new Simulation(id, this.BuildStrategies(strategies));

            this._simulations.Add(simulation);

            return simulation;
        }

        public void Draw(GameTime gameTime)
        {
            foreach (ISimulation simulation in this._simulations)
            {
                simulation.Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (ISimulation simulation in this._simulations)
            {
                simulation.Update(gameTime);
            }
        }

        private IEnumerable<IStrategy> BuildStrategies(StrategyTypeEnum[] strategies)
        {
            List<Type> strategyTypes = [];
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Client && strategies.Contains(StrategyTypeEnum.Predictive))
            {
                strategyTypes.Add(typeof(PredictiveStrategy));
            }
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Client && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                strategyTypes.Add(typeof(LockstepStrategy_Client));
            }
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Server && strategies.Contains(StrategyTypeEnum.Lockstep))
            {
                strategyTypes.Add(typeof(LockstepStrategy_Server));
            }

            foreach (Type type in strategyTypes)
            {
                IStrategy strategy = (IStrategy)this._scenes.Create(type, builder =>
                {
                    builder.RegisterNetScope<IStrategy>(this._netScope.Group.Peer.Type, this._netScope.Group.Id);
                });

                yield return strategy;
            }
        }
    }
}