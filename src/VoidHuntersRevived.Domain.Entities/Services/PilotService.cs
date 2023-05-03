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
    internal sealed class PilotService : IPilotService
    {
        public PilotService(IUserPilotMappingService userPilots)
        {
        }

        public Entity CreatePilot(Entity pilotable, ISimulationEvent @event)
        {
            Entity userPilot = @event.Simulation.CreateEntity(@event.KeyGenerator.Next()).MakePilot(pilotable);

            return userPilot;
        }

        public Entity CreateUserPilot(User user, Entity pilotable, ISimulationEvent @event)
        {
            Entity pilot = this.CreatePilot(pilotable, @event);
            pilot.Attach(user);

            return pilot;
        }
    }
}
