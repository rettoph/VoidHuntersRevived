using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface IPilotService
    {
        public Entity CreatePilot(ParallelKey key, Entity pilotable, ISimulation simulation);
        public Entity CreateUserPilot(ParallelKey key, User user, Entity pilotable, ISimulation simulation);
    }
}
