using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationBuilder(VhId id, StrategiesBuilder strategiesBuilder) : BaseInstanceBuilder<ISimulation>
    {
        public VhId Id = id;
        public StrategiesBuilder StrategiesBuilder = strategiesBuilder;

        protected override Simulation build()
        {
            return new Simulation(this.Id, this.StrategiesBuilder.Build());
        }
    }
}
