using Autofac;
using Guppy.Attributes;
using Guppy.Common.Extensions.Autofac;
using Guppy.StateMachine;
using Guppy.StateMachine.Filters;

namespace VoidHuntersRevived.Common.Simulations.Attributes
{
    public class SimulationFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly SimulationType RequiredSimulationType;

        public SimulationFilterAttribute(SimulationType requiredSimulationType)
        {
            this.RequiredSimulationType = requiredSimulationType;
        }

        protected override void Configure(ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<SimulationType>(classType, new State<SimulationType>(this.RequiredSimulationType)));
        }
    }

    public sealed class SimulationFilterAttribute<TSimulation> : GuppyConfigurationAttribute
        where TSimulation : ISimulation
    {
        protected override void Configure(ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<Type>(classType, new State<Type>(
                key: StateKey<Type>.Create<ISimulation>(),
                value: typeof(TSimulation))));
        }
    }
}
