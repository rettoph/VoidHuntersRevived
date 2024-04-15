using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Filters;

namespace VoidHuntersRevived.Domain.Simulations.Common.Attributes
{
    public class SimulationFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly SimulationType RequiredSimulationType;

        public SimulationFilterAttribute(SimulationType requiredSimulationType)
        {
            this.RequiredSimulationType = requiredSimulationType;
        }

        protected override void Configure(IContainer boot, ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<SimulationType>(classType, new State<SimulationType>(this.RequiredSimulationType)));
        }
    }

    public sealed class SimulationFilterAttribute<TSimulation> : GuppyConfigurationAttribute
        where TSimulation : ISimulation
    {
        protected override void Configure(IContainer boot, ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<Type>(classType, new State<Type>(
                key: StateKey<Type>.Create<ISimulation>(),
                value: typeof(TSimulation))));
        }
    }
}
