using Guppy.Common.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class UserShipMappingService : IUserShipMappingService
    {
        private Map<int, EventId> _userShips;
        private Dictionary<EventId, int> _count;

        public UserShipMappingService()
        {
            _userShips = new Map<int, EventId>();
            _count = new Dictionary<EventId, int>();
        }

        public void Add(int userId, EventId shipKey)
        {
            if(!_userShips.TryAdd(userId, shipKey))
            {
                return;
            }

            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, shipKey, out bool exists);
            count++;
        }

        public void Remove(EventId shipKey)
        {
            if (!_userShips.TryRemove(shipKey))
            {
                return;
            }

            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, shipKey, out bool exists);
            count--;

            if(count == 0)
            {
                _count.Remove(shipKey);
                _userShips.TryRemove(shipKey);
            }
        }

        public int GetUserId(EventId key)
        {
            return _userShips[key];
        }

        public EventId GetShipKey(int userId)
        {
            return _userShips[userId];
        }

        public bool TryGetShipKey(int userId, out EventId key)
        {
            return _userShips.TryGet(userId, out key);
        }
    }
}
