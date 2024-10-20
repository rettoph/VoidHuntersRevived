using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Tests.Domain.Entities.Events;

namespace VoidHuntersRevived.Tests.Domain.Entities.Engines
{
    internal class TestInputEngine : StrategyEngine, IOnInitializeEngine<IStrategy>, IEventEngine<TestSpawnInput>, IEventEngine<TestDepawnInput>
    {
        private IEntitySpawnService _entitySpawnService = null!;

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy simulation)
        {
            _entitySpawnService = simulation.Engines.Get<IEntitySpawnService>();
        }

        public void Process(VhId eventId, TestSpawnInput data)
        {
            if (data.DoDiscard == true && this.Strategy.Type == StrategyTypeEnum.Lockstep)
            {
                return;
            }

            _entitySpawnService.Spawn(eventId.Create(1), data.EntityType.Key, data.EntityId);
        }

        public void Process(VhId eventId, TestDepawnInput data)
        {
            _entitySpawnService.Despawn(eventId.Create(1), data.EntityId);
        }
    }
}
