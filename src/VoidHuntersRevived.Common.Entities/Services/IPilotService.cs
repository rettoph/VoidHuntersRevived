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
        public Entity CreatePilot(Entity pilotable, ISimulationEvent @event);
        public Entity CreateUserPilot(User user, Entity pilotable, ISimulationEvent @event);
    }
}
