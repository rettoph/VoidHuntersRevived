using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IUserPilotService
    {
        public Entity CreateUserPilot(ParallelKey key, User user, Entity pilotable, ISimulation simulation);

        public int GetUserId(ParallelKey key);

        public ParallelKey GetParallelKey(User user);

        public ParallelKey GetParallelKey(int userId);

        public bool TryGetParallelKey(User user, out ParallelKey key);
        public bool TryGetParallelKey(int userId, out ParallelKey key);
    }
}
