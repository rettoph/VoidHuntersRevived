using Guppy.Common.Collections;
using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class UserPilotService : IUserPilotService
    {
        private Map<ParallelKey, int> _userKeys;

        public UserPilotService()
        {
            _userKeys = new Map<ParallelKey, int>();
        }

        public Entity CreateUserPilot(ParallelKey key, User user, Entity pilotable, ISimulation simulation)
        {
            _userKeys.Add(key, user.Id);

            Entity userPilot = simulation.CreateEntity(key).MakePilot(pilotable);
            userPilot.Attach(user);

            return userPilot;
        }

        public int GetUserId(ParallelKey key)
        {
            return _userKeys[key];
        }

        public ParallelKey GetParallelKey(User user)
        {
            return _userKeys[user.Id];
        }

        public ParallelKey GetParallelKey(int userId)
        {
            return _userKeys[userId];
        }

        public bool TryGetParallelKey(User user, out ParallelKey key)
        {
            return _userKeys.TryGet(user.Id, out key);
        }

        public bool TryGetParallelKey(int userId, out ParallelKey key)
        {
            return _userKeys.TryGet(userId, out key);
        }
    }
}
