using Guppy.Network;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations.Extensions
{
    public static class ISimulationExtensions
    {
        private static ConditionalWeakTable<ISimulation, NetScope> _netScopes = new();

        public static NetScope NetScope(this ISimulation simulation)
        {
            if(!_netScopes.TryGetValue(simulation, out NetScope? netScope))
            {
                netScope = simulation.Provider.GetRequiredService<NetScope>();
                _netScopes.Add(simulation, netScope);
            }

            return netScope;
        }
    }
}
