using Guppy.Core.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Tests.Common.Simulations.Strategies;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class StrategiesBuilder : BaseInstanceBuilder<IEnumerable<IStrategy>>
    {
        private readonly IStrategyBuilder[] _builders;

        public StrategiesBuilder(IEnumerable<Type> strategyBuilderTypes, Action<IStrategyBuilder>? configuration = null)
        {
            foreach (Type type in strategyBuilderTypes)
            {
                ThrowIf.Type.IsNotAssignableFrom<IStrategyBuilder>(type);
            }

            _builders = strategyBuilderTypes.Select(x =>
            {
                var builder = (IStrategyBuilder)(Activator.CreateInstance(x) ?? throw new NotImplementedException());
                configuration?.Invoke(builder);
                return builder;
            }).ToArray();
        }

        protected override IEnumerable<IStrategy> build()
        {
            return _builders.Select(x => x.Build());
        }
    }
}