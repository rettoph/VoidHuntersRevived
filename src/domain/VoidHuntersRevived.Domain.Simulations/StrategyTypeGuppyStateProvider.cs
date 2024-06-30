using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations
{
    [AutoLoad]
    internal sealed class StrategyTypeGuppyStateProvider : IStateProvider
    {
        public readonly Lazy<IOptional<IStrategy>>? _strategy;

        public StrategyTypeGuppyStateProvider(ILifetimeScope scope)
        {
            if (scope.IsRoot() == false)
            {
                _strategy = scope.Resolve<Lazy<IOptional<IStrategy>>>();
            }
        }

        public IEnumerable<IState> GetStates()
        {
            yield return new State<Type>(StateKey<Type>.Create<IStrategy>(), _strategy?.Value?.GetType(), (x, y) => x?.IsAssignableTo(y) ?? false);
            yield return new State<StrategyTypeEnum>(_strategy?.Value.Value?.Type ?? StrategyTypeEnum.None, (x, y) => x.HasFlag(y));
        }
    }
}
