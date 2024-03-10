using Autofac;
using Guppy.Attributes;
using Guppy.Extensions.Autofac;
using Guppy.Providers;
using Guppy.StateMachine;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Domain.Simulations
{
    [AutoLoad]
    internal sealed class SimulationTypeGuppyStateProvider : StateProvider
    {
        public readonly ISimulation? _simulation;

        public SimulationTypeGuppyStateProvider(ILifetimeScope scope)
        {
            if (scope.HasTag(nameof(Simulation)))
            {
                _simulation = scope.Resolve<ISimulation>();
            }
        }

        public override IEnumerable<IState> GetStates()
        {
            yield return new State<Type>(StateKey<Type>.Create<ISimulation>(), _simulation?.GetType(), (x, y) => x?.IsAssignableTo(y) ?? false);
            yield return new State<SimulationType>(_simulation?.Type ?? SimulationType.None, (x, y) => x.HasFlag(y));
        }
    }
}
