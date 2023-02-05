using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.DependencyInjection;
using VoidHuntersRevived.Common.Simulations.Filters;

namespace VoidHuntersRevived.Common.Simulations.Attributes
{
    public sealed class SimulationTypeFilterAttribute : InitializableAttribute
    {
        public readonly SimulationType SimulationType;

        public SimulationTypeFilterAttribute(SimulationType simulationType)
        {
            this.SimulationType = simulationType;
        }

        protected override void Initialize(GuppyEngine engine, Type classType)
        {
            engine.Services.AddFilter(new SimulationTypeFilter(this.SimulationType, classType));
        }
    }
}
