using Guppy.Core.Network.Common.Enums;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Tests.Common.Simulations
{
    public class SimulationBuilder(VhId id, PeerType peerType) : BaseInstanceBuilder<StrategyTypeEnum[], Simulation>
    {
        public VhId Id = id;
        public readonly StrategiesFactoryBuilder StrategiesFactoryBuilder = new StrategiesFactoryBuilder(peerType);

        protected override Simulation build(StrategyTypeEnum[] strategies)
        {
            return new Simulation(this.Id, this.StrategiesFactoryBuilder.GetInstance(), strategies);
        }
    }
}
