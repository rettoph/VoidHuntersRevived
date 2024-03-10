using Autofac;
using Guppy.Common;
using Guppy.Messaging;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService
    {
        private bool _configured;
        private bool _initialized;
        private readonly IBus _bus;
        private readonly ILifetimeScope _scope;
        private readonly IDictionary<SimulationType, ISimulation> _simulations;
        private readonly IList<SimulationType> _types;
        private readonly IList<ISimulation> _list;
        private readonly IList<ISimulation> _reversed;

        public ReadOnlyCollection<SimulationType> Types { get; }

        public ReadOnlyCollection<ISimulation> Instances { get; }

        public SimulationType Flags { get; private set; }


        public ISimulation this[SimulationType type] => _simulations[type];

        public SimulationService(IBus bus, ILifetimeScope scope)
        {
            _bus = bus;
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

            IEnumerable<ISimulation> simulations = _scope.Resolve<IFiltered<ISimulation>>().Instances;
            foreach (ISimulation simulation in simulations)
            {
                if (!this.Flags.HasFlag(simulation.Type))
                {
                    continue;
                }

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
