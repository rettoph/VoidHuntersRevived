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

        public Entity CreatePilot(ParallelKey key, Entity pilotable, ISimulation simulation)
        {
            Entity userPilot = simulation.CreateEntity(key).MakePilot(pilotable);

            return userPilot;
        }

        public Entity CreateUserPilot(ParallelKey key, User user, Entity pilotable, ISimulation simulation)
        {
            Entity pilot = this.CreatePilot(key, pilotable, simulation);
            pilot.Attach(user);

            return pilot;
        }
    }
}
