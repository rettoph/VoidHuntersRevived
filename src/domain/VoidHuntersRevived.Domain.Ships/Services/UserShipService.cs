using Guppy.Common.Collections;
using Guppy.Network;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    internal class UserShipService : BasicEngine, IUserShipService,
        IOnSpawnEngine<UserId>,
        IOnDespawnEngine<UserId>,
        IEventEngine<SetUserShipUserId>
    {
        private readonly Map<EntityId, int> _shipVhIdUserIdMap;
        private readonly INetGroup _netScope;
        private readonly IEntityService _entities;

        public UserShipService(INetGroup netScope, IEntityService entities)
        {
            _netScope = netScope;
            _shipVhIdUserIdMap = new Map<EntityId, int>();
            _entities = entities;
        }

        public void OnSpawn(VhId sourceEventId, EntityId id, ref UserId userId, in GroupIndex groupIndex)
        {
            this.Simulation.Publish(sourceEventId, new SetUserShipUserId()
            {
                ShipVhId = id.VhId,
                UserId = userId.Value
            });
        }

        public void OnDespawn(VhId sourceEventId, EntityId id, ref UserId userId, in GroupIndex groupIndex)
        {
            this.Simulation.Publish(sourceEventId, new SetUserShipUserId()
            {
                ShipVhId = id.VhId,
                UserId = userId.Value
            });
        }

        public void Process(VhId eventId, SetUserShipUserId data)
        {
            if (_entities.TryGetId(data.ShipVhId, out EntityId shipId) == false)
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
                this.Simulation.Publish(eventId, new SetUserShipUserId()
                {
                    ShipVhId = shipId.VhId,
                    UserId = null
                });
            }

            _shipVhIdUserIdMap.TryAdd(shipId, data.UserId.Value);
        }

        public void SetUserId(VhId sourceId, EntityId shipId, int? userId)
        {
            this.Simulation.Publish(sourceId, new SetUserShipUserId()
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
            if (_entities.TryGetId(shipVhId, out EntityId shipId))
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
            if (_netScope.Peer?.Users.Current is not null)
            {
                return this.TryGetShipId(_netScope.Peer.Users.Current.Id, out shipId);
            }

            shipId = default;
            return false;
        }
    }
}
