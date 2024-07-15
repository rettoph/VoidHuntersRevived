using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Domain.Entities.Events;

namespace VoidHuntersRevived.Tests.Domain.Entities.Engines
{
    internal class TestInputEngine : StrategyEngine, IEventEngine<TestSpawnInput>, IEventEngine<TestDepawnInput>
    {
        private IEntityQueryService _entities = null!;

        public override void Initialize(IStrategy simulation)
        {
            base.Initialize(simulation);

            _entities = simulation.Engines.Get<IEntityService>();
        }

        public void Process(VhId eventId, TestSpawnInput data)
        {
            if (data.DoDiscard == true && this.Simulation.Type == StrategyTypeEnum.Lockstep)
            {
                return;
            }

            _entities.Spawn(eventId.Create(1), data.EntityType, data.EntityId);
        }

        public void Process(VhId eventId, TestDepawnInput data)
        {
            _entities.Despawn(eventId.Create(1), data.EntityId);
        }
    }
}
