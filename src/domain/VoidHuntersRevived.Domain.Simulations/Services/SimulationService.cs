using System.Collections.ObjectModel;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Services;
using Guppy.Game.Graphics.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    public sealed partial class SimulationService : ISimulationService, IDisposable
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

        public ISimulation Create(VhId id, params StrategyTypeEnum[] strategyTypes)
        {
            List<(Type type, bool graphical)> strategies = [];
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Client && strategyTypes.Contains(StrategyTypeEnum.Predictive))
            {
                strategies.Add((typeof(PredictiveStrategy), true));
            }
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Client && strategyTypes.Contains(StrategyTypeEnum.Lockstep))
            {
                strategies.Add((typeof(LockstepStrategy_Client), false));
            }
            if (this._netScope.Group.Peer.Type == PeerTypeEnum.Server && strategyTypes.Contains(StrategyTypeEnum.Lockstep))
            {
                strategies.Add((typeof(LockstepStrategy_Server), false));
            }

            Simulation simulation = new(id, this.StrategiesBuilder(strategies));
            this._simulations.Add(simulation);
            simulation.Initialize();

            return simulation;
        }

        public ISimulation Create(VhId id, params Type[] strategyTypes)
        {
            List<(Type type, bool graphical)> strategies = [];
            if (strategyTypes.Contains(typeof(PredictiveStrategy)))
            {
                strategies.Add((typeof(PredictiveStrategy), true));
            }
            if (strategyTypes.Contains(typeof(LockstepStrategy_Client)))
            {
                strategies.Add((typeof(LockstepStrategy_Client), false));
            }
            if (strategyTypes.Contains(typeof(LockstepStrategy_Server)))
            {
                strategies.Add((typeof(LockstepStrategy_Server), false));
            }

            Simulation simulation = new(id, this.StrategiesBuilder(strategies));
            this._simulations.Add(simulation);
            simulation.Initialize();

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

        private Func<ISimulation, IEnumerable<IStrategy>> StrategiesBuilder(List<(Type type, bool graphical)> strategies)
        {
            IEnumerable<IStrategy> BuilderMethod(ISimulation simulation)
            {
                foreach ((Type type, bool graphical) in strategies)
                {
                    IStrategy strategy = (IStrategy)this._scenes.Create(type, builder =>
                    {
                        builder.AddGraphicsEnabled(graphical);
                        builder.RegisterNetScope<IStrategy>(this._netScope.Group.Peer.Type, this._netScope.Group.Id);
                        builder.RegisterInstance(simulation).As<ISimulation>();
                    });

                    yield return strategy;
                }
            }

            return BuilderMethod;
        }
    }
}