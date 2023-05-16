using Guppy.Common.Collections;
using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class UserShipMappingService : IUserShipMappingService
    {
        private Map<int, ParallelKey> _userShips;
        private Dictionary<ParallelKey, int> _count;

        public UserShipMappingService()
        {
            _userShips = new Map<int, ParallelKey>();
            _count = new Dictionary<ParallelKey, int>();
        }

        public void Add(int userId, ParallelKey shipKey)
        {
            if(!_userShips.TryAdd(userId, shipKey))
            {
                return;
            }

            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, shipKey, out bool exists);
            count++;
        }

        public void Remove(ParallelKey shipKey)
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

        public int GetUserId(ParallelKey key)
        {
            return _userShips[key];
        }

        public ParallelKey GetShipKey(int userId)
        {
            return _userShips[userId];
        }

        public bool TryGetShipKey(int userId, out ParallelKey key)
        {
            return _userShips.TryGet(userId, out key);
        }
    }
}
