using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Common.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Services;

namespace VoidHuntersRevived.Common.Filters
{
    public sealed class SimulationTypeFilter : SimpleFilter
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilter(SimulationType simulationType, Type type) : base(type)
        {
            this.SimulationType = simulationType;
        }

        public override bool Invoke(IServiceProvider provider, IServiceConfiguration service)
        {
            var simulations = provider.GetRequiredService<ISimulationService>();

            return simulations.Contains(this.SimulationType);
        }
    }
}
