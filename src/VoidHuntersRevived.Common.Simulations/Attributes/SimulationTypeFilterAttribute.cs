using Guppy.Attributes;
using Guppy.Common.Filters;
using Guppy.Configurations;
using Microsoft.Extensions.DependencyInjection;

namespace VoidHuntersRevived.Common.Simulations.Attributes
{
    public sealed class SimulationTypeFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilterAttribute(SimulationType simulationType)
        {
            this.SimulationType = simulationType;
        }

        protected override void Configure(GuppyConfiguration configuration, Type classType)
        {
            configuration.Services.AddFilter(new ServiceFilter<SimulationType>(classType, this.SimulationType));
        }
    }
}
