using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Common.Filters;
using Guppy.Configurations;

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
            configuration.Builder.AddFilter(new ServiceFilter<SimulationType>(classType, this.SimulationType));
        }
    }
}
