using Autofac;
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
        public readonly Lazy<ISimulation>? _simulation;

        public SimulationTypeGuppyStateProvider(ILifetimeScope scope)
        {
            if (scope.HasTag(nameof(Simulation)))
            {
                _simulation = scope.Resolve<Lazy<ISimulation>>();
            }
        }

        public IEnumerable<IState> GetStates()
        {
            yield return new State<Type>(StateKey<Type>.Create<ISimulation>(), _simulation?.GetType(), (x, y) => x?.IsAssignableTo(y) ?? false);
            yield return new State<SimulationType>(_simulation?.Value.Type ?? SimulationType.None, (x, y) => x.HasFlag(y));
        }
    }
}
