﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserPilotSystem : BasicSystem,
        ISubscriber<IEvent<PlayerAction>>
    {
        private readonly NetScope _scope;
        private readonly ILogger _logger;

        public UserPilotSystem(NetScope scope, ILogger logger)
        {
            _scope = scope;
            _logger = logger;
        }

        public void Process(in IEvent<PlayerAction> message)
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

            var ship = simulation.CreateShip(ParallelKey.From(ParallelTypes.Ship, user), ShipParts.HullSquare);
            var pilot = simulation.CreatePilot(ParallelKey.From(ParallelTypes.Pilot, user.Id), ship);

            var chain = simulation.CreateChain(ParallelKey.From(ParallelTypes.Chain, user), ShipParts.HullSquare);
        }
    }
}
