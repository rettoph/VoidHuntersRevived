using Autofac;
using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions.Autofac;
using Guppy.Core.StateMachine.Common;
using Guppy.Core.StateMachine.Common.Providers;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations
{
    [AutoLoad]
    internal sealed class SimulationTypeGuppyStateProvider : IStateProvider
    {
        public readonly Lazy<IOptional<ISimulation>>? _simulation;

        public SimulationTypeGuppyStateProvider(ILifetimeScope scope)
        {
            if (scope.IsRoot() == false)
            {
                _simulation = scope.Resolve<Lazy<IOptional<ISimulation>>>();
            }
        }

        public IEnumerable<IState> GetStates()
        {
            yield return new State<Type>(StateKey<Type>.Create<ISimulation>(), _simulation?.Value?.GetType(), (x, y) => x?.IsAssignableTo(y) ?? false);
            yield return new State<SimulationType>(_simulation?.Value.Value?.Type ?? SimulationType.None, (x, y) => x.HasFlag(y));
        }
    }
}
