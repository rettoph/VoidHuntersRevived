using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using static VoidHuntersRevived.Library.Simulations.Lockstep.Providers.ISimulationStateProvider;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    internal sealed class SimulationStateProvider : ISimulationStateProvider
    {
        private readonly Dictionary<ISimulation, LockstepData> _items = new();
        public IEnumerable<LockstepData> Items => _items.Values;

        public void Add(ISimulation simulation, IServiceProvider provider)
        {
            _items.Add(simulation, new LockstepData(
                simulation: simulation, 
                state: provider.GetRequiredService<State>(),
                scope: provider.GetRequiredService<NetScope>()));
        }

        public void Remove(ISimulation simulation)
        {
            _items.Remove(simulation);
        }
    }
}
