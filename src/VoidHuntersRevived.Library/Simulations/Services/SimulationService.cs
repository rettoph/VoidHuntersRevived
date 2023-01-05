using Guppy.Common;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Library.Simulations.Services
{
    internal sealed partial class SimulationService : ISimulationService
    {
        private bool _initialized;
        private byte _typeFlags;
        private readonly IBus _bus;
        private readonly IServiceProvider _provider;
        private readonly IDictionary<SimulationType, ISimulation> _simulations;
        private readonly IList<SimulationType> _types;
        private readonly IList<ISimulation> _list;

        public ReadOnlyCollection<SimulationType> Types { get; }

        public ReadOnlyCollection<ISimulation> Instances { get; }

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
        }

        public void Initialize(params SimulationType[] simulationTypes)
        {
            if (_initialized)
            {
                throw new InvalidOperationException();
            }

            _typeFlags = simulationTypes.Select(x => x.Flag).Aggregate((f1, f2) => (byte)(f1 | f2));


            var simulations = _provider.GetRequiredService<IFiltered<ISimulation>>().Instances;
            foreach (var simulation in simulations)
            {
                _simulations.Add(simulation.Type, simulation);
                _list.Add(simulation);
                _types.Add(simulation.Type);
            }

            foreach(var simulation in simulations)
            {
                simulation.Initialize(_provider);
            }

            _initialized = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach (ISimulation simulation in _simulations.Values)
            {
                simulation.Update(gameTime);
            }
        }

        public void PublishEvent(PeerType source, ISimulationData data)
        {
            foreach(ISimulation simulation in _simulations.Values)
            {
                simulation.PublishEvent(source, data);
            }
        }

        public bool Contains(SimulationType type)
        {
            return (_typeFlags & type.Flag) != 0;
        }
    }
}
