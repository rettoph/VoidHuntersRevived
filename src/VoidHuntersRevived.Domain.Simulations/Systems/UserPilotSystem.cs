using Guppy.Attributes;
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
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Domain.Entities.Components;

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

            var key = ParallelKey.From(ParallelTypes.Ship, user);

            if(simulation.HasEntity(key))
            { // This operation has already been done
                return;
            }

            var ship = simulation.CreateShip(key, ShipParts.HullSquare);
            var pilot = simulation.CreatePilot(ParallelKey.From(ParallelTypes.Pilot, user.Id), ship);

            var chain = simulation.CreateChain(ParallelKey.From(ParallelTypes.Chain, key), ShipParts.HullSquare);

            var piece = simulation.CreateShipPart(ParallelKey.From(ParallelTypes.ShipPart, key, 1), ShipParts.HullSquare);
            simulation.PublishEvent(new CreateJointing()
            {
                Parent = ship.Get<Ship>().Bridge.Get<Parallelable>().Key,
                ParentJointId = 1,
                Child = piece.Get<Parallelable>().Key,
                ChildJointId = 0
            });
        }
    }
}
