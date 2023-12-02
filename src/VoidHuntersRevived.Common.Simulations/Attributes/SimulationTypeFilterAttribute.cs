using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Common.Filters;

namespace VoidHuntersRevived.Common.Simulations.Attributes
{
    public sealed class SimulationTypeFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilterAttribute(SimulationType simulationType)
        {
            this.SimulationType = simulationType;
        }

        protected override void Configure(ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new ServiceFilter<SimulationType>(classType, this.SimulationType));
        }
    }
}
