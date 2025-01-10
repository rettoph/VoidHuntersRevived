using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    internal class UserIdEngine(IEntityQueryService entityQueryService) : StrategyEngine,
        IOnSpawnEngine<UserId>,
        IOnDespawnEngine<UserId>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<UserId> userId)
        {
            if (userId.Component.Value is null)
            {
                return;
            }

            this._entityQueryService.GetFilter<EntityLocalId, IUser>(userId.Component.Value.Value).Add(userId);
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<UserId> userId)
        {
            throw new NotImplementedException();
        }
    }
}