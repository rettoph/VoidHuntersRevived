using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Tests.Domain.Entities.Events;

namespace VoidHuntersRevived.Tests.Domain.Entities.Systems
{
    public class TestInputSystem : ISceneSystem, IOnInitializeSystem<IStrategy>, IEventSystem<TestSpawnInput>, IEventSystem<TestDepawnInput>
    {
        private IEntitySpawnService _entitySpawnService = null!;

        [SequenceGroup<OnInitializeSequenceGroupEnum>(OnInitializeSequenceGroupEnum.Initialize)]
        public void OnInitialize(IStrategy simulation)
        {
            this._entitySpawnService = simulation.Resolve<IEntitySpawnService>();
        }

        public void Process(VhId eventId, TestSpawnInput data)
        {
            this._entitySpawnService.Spawn(eventId.Create(1), data.EntityTemplateKey, data.EntityGlobalId);
        }

        public void Process(VhId eventId, TestDepawnInput data)
        {
            this._entitySpawnService.Despawn(eventId.Create(1), data.EntityGlobalId);
        }
    }
}