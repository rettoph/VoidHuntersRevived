using System.Collections.ObjectModel;
using Guppy.Core.Common.Extensions;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Game.Common.Services;
using Guppy.Game.Graphics.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Strategies;

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
            if (strategyTypes.Contains(StrategyTypeEnum.Predictive))
            {
                strategies.Add((typeof(PredictiveStrategy), true));
            }
            if (strategyTypes.Contains(StrategyTypeEnum.Lockstep))
            {
                strategies.Add((typeof(LockstepStrategy), false));
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
            if (strategyTypes.Contains(typeof(LockstepStrategy)))
            {
                strategies.Add((typeof(LockstepStrategy), false));
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
                        builder.Variables.AddGraphicsEnabled(graphical);
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