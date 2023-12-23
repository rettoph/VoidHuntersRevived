using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Extensions.Autofac;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Simulations
{
    [AutoLoad]
    internal sealed class SimulationState : State
    {
        public readonly ISimulation? _simulation;

        public SimulationState(ILifetimeScope scope)
        {
            if (scope.HasTag(nameof(Simulation)))
            {
                _simulation = scope.Resolve<ISimulation>();
            }

        }

        public override bool Matches(object? value)
        {
            if (_simulation is null)
            {
                return false;
            }

            if (value is Type simulationType)
            {
                return _simulation.GetType().IsAssignableTo(simulationType);
            }

            if (value is SimulationType simulationTypeEnum)
            {
                return _simulation.Type == simulationTypeEnum;
            }

            return false;
        }
    }
}
