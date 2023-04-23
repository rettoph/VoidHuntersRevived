using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities;
using VoidHuntersRevived.Common.Simulations.Components;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserPilotSystem : BasicSystem,
        ISubscriber<IEvent<UserJoined>>
    {
        private readonly NetScope _scope;
        private readonly ILogger _logger;
        private readonly IShipPartService _shipParts;
        private readonly IChainService _chains;
        private readonly IShipService _ships;
        private readonly INodeService _nodeService;
        private ComponentMapper<Node> _nodes;

        public UserPilotSystem(NetScope scope, IShipPartService shipParts, IChainService chains, IShipService ships, INodeService nodeService, ILogger logger)
        {
            _scope = scope;
            _shipParts = shipParts;
            _chains = chains;
            _ships = ships;
            _nodeService = nodeService;
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

            Entity ship = _ships.CreateShip(key.Create(ParallelTypes.Ship), ShipParts.HullSquare, @event.Target);
            
            // TODO: Make a CreatePilot event and override this extension's functionality
            var pilot = @event.Target.CreatePilot(key, ship);

            Entity chain = _chains.CreateChain(ParallelTypes.Chain.Create(1337), ShipParts.HullSquare, Vector2.Zero, 0, @event.Target);
        }
    }
}
