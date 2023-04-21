using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Identity;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations.Components;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserPilotSystem : BasicSystem,
        ISubscriber<IEvent<UserJoined>>
    {
        private readonly NetScope _scope;
        private readonly ILogger _logger;
        private ComponentMapper<Node> _nodes;

        public UserPilotSystem(NetScope scope, ILogger logger)
        {
            _scope = scope;
            _logger = logger;

            _nodes = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _nodes = world.ComponentMapper.GetMapper<Node>();
        }

        public void Process(in IEvent<UserJoined> @event)
        {
            var user = _scope.Peer!.Users.UpdateOrCreate(@event.Data.Id, @event.Data.Claims);

            // Ensure the user has been added to the scope
            if (!_scope.Users.TryGet(user.Id, out _))
            {
                _scope.Users.Add(user);
            }

            var key = user.GetKey();

            if (@event.Target.HasEntity(key))
            { // This operation has already been done
                return;
            }

            var ship = @event.Target.CreateShip(@event, key.Create(ParallelTypes.Ship), ShipParts.HullSquare);
            
            // TODO: Make a CreatePilot event and override this extension's functionality
            var pilot = @event.Target.CreatePilot(key, ship);

            var piece = @event.Target.CreateShipPart(key.Create(ParallelTypes.ShipPart), ShipParts.HullSquare);
            @event.PublishConsequent(new CreateJointing()
            {
                Parent = ship.Get<Ship>().Bridge.Get<Parallelable>().Key,
                ParentJointId = 1,
                Joint = piece.Get<Parallelable>().Key,
                JointId = 0
            });

            var chain = @event.Target.CreateChain(@event, ParallelTypes.Chain.Create(1337), ShipParts.HullSquare);
        }
    }
}
