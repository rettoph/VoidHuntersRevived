using Guppy.Common;
using Guppy.Network.Enums;
using LiteNetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        private readonly IBus _bus;
        private readonly IServiceProvider _provider;
        private readonly IDictionary<SimulationType, ISimulation> _simulations;

        public SimulationType Flags { get; private set; }

        public IEnumerable<ISimulation> Instances => _simulations.Values;

        public ISimulation this[SimulationType type] => _simulations[type];

        public SimulationService(IBus bus, IServiceProvider provider)
        {
            _bus = bus;
            _provider = provider;
            _simulations = new Dictionary<SimulationType, ISimulation>();

            Flags = 0;
        }

        public void Initialize(SimulationType flags)
        {
            if (_initialized)
            {
                throw new InvalidOperationException();
            }

            this.Flags = flags;

            var simulations = _provider.GetRequiredService<IFiltered<ISimulation>>().Instances;
            foreach (var simulation in simulations)
            {
                _simulations.Add(simulation.Type, simulation);
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
    }
}
