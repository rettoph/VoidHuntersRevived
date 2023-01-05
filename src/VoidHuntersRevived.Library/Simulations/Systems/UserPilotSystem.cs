using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Library.Constants;
using VoidHuntersRevived.Library.Simulations.Events;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    [GuppyFilter<GameGuppy>()]
    internal sealed class UserPilotSystem : BasicSystem,
        ISubscriber<ISimulationEvent<PlayerAction>>
    {
        private readonly NetScope _scope;

        public UserPilotSystem(NetScope scope)
        {
            _scope = scope;
        }

        public void Process(in ISimulationEvent<PlayerAction> message)
        {
            var user = _scope.Peer!.Users.UpdateOrCreate(message.Data.UserAction.Id, message.Data.UserAction.Claims);

            switch (message.Data.UserAction.Action)
            {
                case Guppy.Network.Messages.UserAction.Actions.UserJoined:
                    this.ConfigureNewUser(message.Simulation, user);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ConfigureNewUser(ISimulation simulation, User user)
        {
            // Ensure the user has been added to the scope
            if(!_scope.Users.TryGet(user.Id, out _))
            {
                _scope.Users.Add(user);
            }

            var pilot = simulation.CreatePilot(ParallelKey.From(ParallelTypes.Pilot, user.Id));
            var ship = simulation.CreateShip(ParallelKey.From(ParallelTypes.Ship, user));

            // Bind the pilot to the new pilotable ship
            pilot.Get<Piloting>().Pilotable = ship.Get<Pilotable>();
        }
    }
}
