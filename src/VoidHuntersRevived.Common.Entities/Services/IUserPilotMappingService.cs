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
    public interface IUserPilotMappingService
    {
        void Add(int userId, ParallelKey pilotKey);

        void Remove(ParallelKey pilotKey);

        public int GetUserId(ParallelKey key);

        public ParallelKey GetPilotKey(int userId);

        public bool TryGetPilotKey(int userId, out ParallelKey key);
    }
}
