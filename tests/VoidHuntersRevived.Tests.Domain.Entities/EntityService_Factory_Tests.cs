using Autofac;
using Autofac.Extras.Moq;
using Guppy.Core.Network.Common.Enums;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Common;
using VoidHuntersRevived.Tests.Domain.Simulations.Common.Extensions;

namespace VoidHuntersRevived.Tests.Domain.Entities
{
    public class EntityService_Factory_Tests
    {
        [Fact]
        public void EntityService_SpawnDespawn_SimulationSynchronizationStress()
        {
            using AutoMock autofac = AutoMockBuilder.Create().RegisterSimulation(VhId.Empty, PeerType.Client, new()
            {
                { StrategyTypeEnum.Predictive, [] },
                { StrategyTypeEnum.Lockstep, [] }
            }).Build();

            ISimulation simulation = autofac.Container.Resolve<ISimulation>();
        }
    }
}