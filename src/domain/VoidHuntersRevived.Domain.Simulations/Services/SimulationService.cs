using Autofac;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Common.Services;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Predictive;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService, IDisposable
    {
        private bool _configured;
        private bool _initialized;
        private readonly ILifetimeScope _scope;
        private readonly IDictionary<SimulationType, ISimulation> _simulations;
        private readonly IList<SimulationType> _types;
        private readonly IList<ISimulation> _list;
        private readonly IList<ISimulation> _reversed;

        public ReadOnlyCollection<SimulationType> Types { get; }

        public ReadOnlyCollection<ISimulation> Instances { get; }

        public SimulationType Flags { get; private set; }


        public ISimulation this[SimulationType type] => _simulations[type];

        public SimulationService(ILifetimeScope scope)
        {
            _scope = scope;
            _simulations = new Dictionary<SimulationType, ISimulation>();
            _list = new List<ISimulation>();
            _types = new List<SimulationType>();
            _reversed = new List<ISimulation>();
            this.Instances = new ReadOnlyCollection<ISimulation>(_list);
            this.Types = new ReadOnlyCollection<SimulationType>(_types);
            this.Flags = 0;
        }

        public void Configure(SimulationType simulationTypeFlags)
        {
            if (_configured || _initialized)
            {
                throw new InvalidOperationException();
            }

            this.Flags = simulationTypeFlags;

            INetScope<ISimulation> netScope = _scope.Resolve<INetScope<ISimulation>>();
            List<Type> simulationTypes = new List<Type>();
            if (netScope.Group.Peer.Type == PeerType.Client && this.Flags.HasFlag(SimulationType.Predictive))
            {
                simulationTypes.Add(typeof(PredictiveSimulation));
            }
            if (netScope.Group.Peer.Type == PeerType.Client && this.Flags.HasFlag(SimulationType.Lockstep))
            {
                simulationTypes.Add(typeof(LockstepSimulation_Client));
            }
            if (netScope.Group.Peer.Type == PeerType.Server && this.Flags.HasFlag(SimulationType.Lockstep))
            {
                simulationTypes.Add(typeof(LockstepSimulation_Server));
            }

            ISceneService scenes = _scope.Resolve<ISceneService>();
            foreach (Type simulationType in simulationTypes)
            {
                ISimulation simulation = (ISimulation)scenes.Create(simulationType, builder =>
                {
                    builder.RegisterInstance(this).As<ISimulationService>();
                    builder.RegisterNetScope<ISimulation>(netScope.Group.Peer.Type, netScope.Group.Id);
                });

                _simulations.Add(simulation.Type, simulation);
                _list.Add(simulation);
                _types.Add(simulation.Type);
                _reversed.Insert(0, simulation);
            }

            _configured = true;
        }

        public void Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException();
            }

            if (_configured == false)
            {
                throw new InvalidOperationException($"{nameof(SimulationService)}::{nameof(Initialize)} - Ensure {nameof(Configure)} is called before running {nameof(Initialize)}");
            }

            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Initialize(this);
            }

            _initialized = true;
        }

        public void Dispose()
        {
            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Dispose();
            }
        }

        public ISimulation First(params SimulationType[] types)
        {
            foreach (SimulationType type in types)
            {
                if (_simulations.TryGetValue(type, out var simulation))
                {
                    return simulation;
                }
            }

            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            foreach (ISimulation simulation in _reversed)
            {
                simulation.Draw(gameTime);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (ISimulation simulation in _list)
            {
                simulation.Update(gameTime);
            }
        }

        public void Input(VhId sourceId, IInputData data)
        {
            foreach (ISimulation simulation in _list)
            {
                simulation.Input(sourceId, data);
            }
        }
    }
}
