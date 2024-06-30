using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Filters;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Attributes
{
    public class StrategyFilterAttribute : GuppyConfigurationAttribute
    {
        public readonly StrategyTypeEnum RequiredStrategyType;

        public StrategyFilterAttribute(StrategyTypeEnum requiredSimulationType)
        {
            this.RequiredStrategyType = requiredSimulationType;
        }

        protected override void Configure(IContainer boot, ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<StrategyTypeEnum>(classType, new State<StrategyTypeEnum>(this.RequiredStrategyType)));
        }
    }

    public sealed class SimulationFilterAttribute<TSimulation> : GuppyConfigurationAttribute
        where TSimulation : IStrategy
    {
        protected override void Configure(IContainer boot, ContainerBuilder builder, Type classType)
        {
            builder.RegisterFilter(new StateServiceFilter<Type>(classType, new State<Type>(
                key: StateKey<Type>.Create<IStrategy>(),
                value: typeof(TSimulation))));
        }
    }
}
