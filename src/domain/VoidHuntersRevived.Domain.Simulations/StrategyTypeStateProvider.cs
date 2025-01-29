using Guppy.Core.Common;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Extensions;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations
{
    internal sealed class StrategyTypeStateProvider : BaseStateProvider
    {
        private readonly Lazy<IOptional<IStrategy>>? _strategy;

        public StrategyTypeStateProvider(IGuppyScope scope)
        {
            if (scope.GetScopeType() == GuppyScopeTypeEnum.Child)
            {
                this._strategy = scope.ResolveService<Lazy<IOptional<IStrategy>>>();
            }
        }

        public override bool TryGet(IStateKey key, out object? state)
        {
            switch (key)
            {
                case IStateKey<StrategyTypeEnum> { Value: StateKey.DefaultValue }:
                    state = this._strategy?.Value.Value?.Type ?? StrategyTypeEnum.None;
                    return true;
                case IStateKey<Type> { Value: nameof(IStrategy) }:
                    state = this._strategy?.Value?.Value?.GetType();
                    return true;
                default:
                    state = null;
                    return false;
            }
        }
    }
}