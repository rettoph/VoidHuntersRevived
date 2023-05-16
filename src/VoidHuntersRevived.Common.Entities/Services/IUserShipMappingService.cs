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
    public interface IUserShipMappingService
    {
        void Add(int userId, ParallelKey shipKey);

        void Remove(ParallelKey shipKey);

        public int GetUserId(ParallelKey shipKey);

        public ParallelKey GetShipKey(int userId);

        public bool TryGetShipKey(int userId, out ParallelKey shipKey);
    }
}
