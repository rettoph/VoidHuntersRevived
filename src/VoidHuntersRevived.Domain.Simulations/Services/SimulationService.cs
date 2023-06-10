﻿using Guppy.Common;
using Guppy.Network.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService
    {
        private bool _initialized;
        private readonly IBus _bus;
        private readonly IServiceProvider _provider;
        private readonly IDictionary<SimulationType, ISimulation> _simulations;
        private readonly IList<SimulationType> _types;
        private readonly IList<ISimulation> _list;

        public ReadOnlyCollection<SimulationType> Types { get; }

        public ReadOnlyCollection<ISimulation> Instances { get; }

        public SimulationType Flags { get; private set; }


        public ISimulation this[SimulationType type] => _simulations[type];

        public SimulationService(IBus bus, IServiceProvider provider)
        {
            _bus = bus;
            _provider = provider;
            _simulations = new Dictionary<SimulationType, ISimulation>();
            _list = new List<ISimulation>();
            _types = new List<SimulationType>();
            this.Instances = new ReadOnlyCollection<ISimulation>(_list);
            this.Types = new ReadOnlyCollection<SimulationType>(_types);
            this.Flags = 0;
        }

        public void Configure(SimulationType simulationTypeFlags)
        {
            if (_initialized)
            {
                throw new InvalidOperationException();
            }

            this.Flags = simulationTypeFlags;

            IEnumerable<ISimulation> simulations = _provider.GetRequiredService<IFiltered<ISimulation>>().Instances;
            foreach (ISimulation simulation in simulations)
            {
                if(!this.Flags.HasFlag(simulation.Type))
                {
                    continue;
                }

                _simulations.Add(simulation.Type, simulation);
                _list.Add(simulation);
                _types.Add(simulation.Type);
            }
        }

        public void Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException();
            }

            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Initialize(this);
            }

            _initialized = true;
        }

        public ISimulation First(params SimulationType[] types)
        {
            foreach(SimulationType type in types)
            {
                if(_simulations.TryGetValue(type, out var simulation))
                {
                    return simulation;
                }
            }

            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Update(gameTime);
            }
        }

        public void Enqueue(EventDto input)
        {

        }

        public void Enqueue(IEventData data)
        {
            EventDto @event = new EventDto()
            {
                Id = EventId.NewId(),
                Data = data
            };

            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Enqueue(@event);
            }
        }
    }
}
