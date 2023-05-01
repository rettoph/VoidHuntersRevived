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
    internal sealed class UserPilotMappingService : IUserPilotMappingService
    {
        private Map<int, ParallelKey> _usersPilots;
        private Dictionary<ParallelKey, int> _count;

        public UserPilotMappingService()
        {
            _usersPilots = new Map<int, ParallelKey>();
            _count = new Dictionary<ParallelKey, int>();
        }

        public void Add(int userId, ParallelKey pilotKey)
        {
            if(!_usersPilots.TryAdd(userId, pilotKey))
            {
                return;
            }

            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, pilotKey, out bool exists);
            count++;
        }

        public void Remove(ParallelKey pilotKey)
        {
            if (!_usersPilots.TryRemove(pilotKey))
            {
                return;
            }

            ref int count = ref CollectionsMarshal.GetValueRefOrAddDefault(_count, pilotKey, out bool exists);
            count--;

            if(count == 0)
            {
                _count.Remove(pilotKey);
                _usersPilots.TryRemove(pilotKey);
            }
        }

        public int GetUserId(ParallelKey key)
        {
            return _usersPilots[key];
        }

        public ParallelKey GetPilotKey(int userId)
        {
            return _usersPilots[userId];
        }

        public bool TryGetPilotKey(int userId, out ParallelKey key)
        {
            return _usersPilots.TryGet(userId, out key);
        }
    }
}
