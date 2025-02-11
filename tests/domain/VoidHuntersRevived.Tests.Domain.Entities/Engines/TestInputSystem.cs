using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Tests.Domain.Entities.Events;

namespace VoidHuntersRevived.Tests.Domain.Entities.Systems
{
    public class TestInputSystem(IEntitySpawnService entitySpawnService) : ISceneSystem, IEventSystem<TestSpawnInput>, IEventSystem<TestDepawnInput>
    {
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, TestSpawnInput data)
        {
            this._entitySpawnService.Spawn(eventId.Create(1), data.EntityTemplateKey, data.EntityGlobalId);
        }

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId eventId, TestDepawnInput data)
        {
            this._entitySpawnService.Despawn(eventId.Create(1), data.EntityGlobalId);
        }
    }
}