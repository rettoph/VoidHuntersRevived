using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Games;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Services
{
    internal sealed class SimulationService : ISimulationService
    {
        private bool _initialized;
        private readonly IServiceProvider _provider;
        private readonly IDictionary<SimulationType, ISimulation> _instances;

        public SimulationType Flags { get; private set; }

        public IEnumerable<ISimulation> Instances => _instances.Values;

        public ISimulation this[SimulationType type] => _instances[type];

        public SimulationService(IServiceProvider provider)
        {
            _provider = provider;
            _instances = new Dictionary<SimulationType, ISimulation>();

            this.Flags = SimulationType.None;
        }

        public void Initialize(SimulationType flags)
        {
            if(_initialized)
            {
                throw new InvalidOperationException();
            }

            this.Flags = flags;

            if (flags.HasFlag(SimulationType.Lockstep))
            {
                _instances.Add(SimulationType.Lockstep, _provider.GetRequiredService<LockstepSimulation>());
            }

            if (flags.HasFlag(SimulationType.Predictive))
            {
                _instances.Add(SimulationType.Predictive, _provider.GetRequiredService<PredictiveSimulation>());
            }

            _initialized = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach(ISimulation simulation in _instances.Values)
            {
                simulation.Update(gameTime);
            }
        }
    }
}
