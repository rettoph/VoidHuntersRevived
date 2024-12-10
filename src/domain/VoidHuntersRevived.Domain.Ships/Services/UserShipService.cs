using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Collections;
using Guppy.Core.Network.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    internal class UserShipService(INetScope<IStrategy> netScope, IEntityQueryService entityQueryService) : StrategyEngine, IUserShipService,
        IOnSpawnEngine<UserId>,
        IOnDespawnEngine<UserId>,
        IEventEngine<SetUserShipUserId>
    {
        private readonly Map<EntityId, int> _shipVhIdUserIdMap = new();
        private readonly INetScope<IStrategy> _netScope = netScope;
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        [SequenceGroup<OnSpawnSequenceGroupEnum>(OnSpawnSequenceGroupEnum.Group03)]
        public void OnSpawn(VhId sourceEventId, IEntityTemplate template, ref Entity<UserId> userId)
        {
            this.Strategy.Publish(sourceEventId, new SetUserShipUserId()
            {
                ShipVhId = userId.GlobalId.Value,
                UserId = userId.Value.Value
            });
        }

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<UserId> userId)
        {
            this.Strategy.Publish(sourceEventId, new SetUserShipUserId()
            {
                ShipVhId = userId.GlobalId.Value,
                UserId = userId.Value.Value
            });
        }

        public void Process(VhId eventId, SetUserShipUserId data)
        {
            if (_entityQueryService.TryGetId(data.ShipVhId, out EntityId shipId) == false)
            {
                throw new Exception();
            }

            if (data.UserId is null)
            {
                _shipVhIdUserIdMap.TryRemove(shipId);
                return;
            }

            if (this.TryGetShipId(data.UserId.Value, out EntityId oldShipId))
            {
                this.Strategy.Publish(eventId, new SetUserShipUserId()
                {
                    ShipVhId = shipId.VhId,
                    UserId = null
                });
            }

            _shipVhIdUserIdMap.TryAdd(shipId, data.UserId.Value);
        }

        public void SetUserId(VhId sourceId, EntityId shipId, int? userId)
        {
            this.Strategy.Publish(sourceId, new SetUserShipUserId()
            {
                ShipVhId = shipId.VhId,
                UserId = userId
            });
        }

        public bool TryGetShipId(int userId, out EntityId shipId)
        {
            return _shipVhIdUserIdMap.TryGet(userId, out shipId);
        }

        public bool TryGetUserId(VhId shipVhId, out int userId)
        {
            if (_entityQueryService.TryGetId(shipVhId, out EntityId shipId))
            {
                return this.TryGetUserId(shipId, out userId);
            }

            userId = default;
            return false;
        }

        public bool TryGetUserId(EntityId shipId, out int userId)
        {
            return _shipVhIdUserIdMap.TryGet(shipId, out userId);
        }

        public bool TryGetCurrentUserShipId(out EntityId shipId)
        {
            if (_netScope.Group.Peer?.Users.Current is not null)
            {
                return this.TryGetShipId(_netScope.Group.Peer.Users.Current.Id, out shipId);
            }

            shipId = default;
            return false;
        }
    }
}
