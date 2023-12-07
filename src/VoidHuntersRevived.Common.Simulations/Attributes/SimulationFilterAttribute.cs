using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.Common.Filters;

namespace VoidHuntersRevived.Common.Simulations.Attributes
{
    public class SimulationFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly object State;

        public SimulationFilterAttribute(SimulationType simulationType)
        {
            this.State = simulationType;
        }
        protected internal SimulationFilterAttribute(Type type)
        {
            this.State = type;
        }

        protected override void Configure(ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new ServiceFilter(classType, this.State));
        }
    }

    public sealed class SimulationFilterAttribute<TSimulation> : SimulationFilterAttribute
        where TSimulation : ISimulation
    {
        public SimulationFilterAttribute() : base(typeof(TSimulation))
        {
        }
    }
}
