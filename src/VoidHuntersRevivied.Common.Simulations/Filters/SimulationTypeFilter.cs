using Guppy.Common.DependencyInjection.Interfaces;
using Guppy.Common.Filters;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Common.Simulations.Filters
{
    public sealed class SimulationTypeFilter : SimpleFilter
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilter(SimulationType simulationType, Type type) : base(type)
        {
            this.SimulationType = simulationType;
        }

        public override bool Invoke(IServiceProvider provider, object service)
        {
            var simulations = provider.GetRequiredService<ISimulationService>();

            return simulations.Flags.HasFlag(this.SimulationType);
        }
    }
}
