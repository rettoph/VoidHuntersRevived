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
    internal sealed class StrategyTypeStateProvider : BaseStateProvider
    {
        public readonly Lazy<IOptional<IStrategy>>? _strategy;

        public StrategyTypeStateProvider(ILifetimeScope scope)
        {
            if (scope.IsRoot() == false)
            {
                _strategy = scope.Resolve<Lazy<IOptional<IStrategy>>>();
            }
        }

        public override bool TryGet(IStateKey key, out object? state)
        {
            switch (key)
            {
                case IStateKey<StrategyTypeEnum> { Value: StateKey.DefaultValue }:
                    state = _strategy?.Value.Value?.Type ?? StrategyTypeEnum.None;
                    return true;
                case IStateKey<Type> { Value: nameof(IStrategy) }:
                    state = _strategy?.Value?.Value?.GetType();
                    return true;
                default:
                    state = null;
                    return false;
            }
        }
    }
}
