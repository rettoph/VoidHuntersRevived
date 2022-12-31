using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Common.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations;

namespace VoidHuntersRevived.Library.Filters
{
    public sealed class SimulationFilter : SimpleFilter
    {
        private SimulationType _simulation;

        public SimulationFilter(SimulationType simulation, Type type) : base(type)
        {
            _simulation = simulation;
        }

        public override bool Invoke(IServiceProvider provider, IServiceConfiguration service)
        {
            var simulations = provider.GetRequiredService<ISimulationService>();

            var result = simulations.Flags.HasFlag(_simulation);

            return result;
        }
    }
}
