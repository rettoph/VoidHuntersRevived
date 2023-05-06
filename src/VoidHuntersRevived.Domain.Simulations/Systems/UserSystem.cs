using Guppy.Attributes;
using Guppy.Network;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Domain.Entities;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities.Services;
using Guppy.Network.Identity;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserSystem : BasicSystem,
        ISimulationEventListener<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly ILogger _logger;
        private readonly IShipPartService _shipParts;
        private readonly IChainService _chains;
        private readonly IShipService _ships;
        private readonly INodeService _nodeService;
        private readonly IPilotService _pilots;
        private ComponentMapper<Node> _nodes;

        public UserSystem(
            NetScope scope, 
            IShipPartService shipParts, 
            IChainService chains, 
            IShipService ships, 
            INodeService nodeService,
            IPilotService pilots,
            ILogger logger)
        {
            _scope = scope;
            _shipParts = shipParts;
            _chains = chains;
            _ships = ships;
            _nodeService = nodeService;
            _pilots = pilots;
            _logger = logger;

            _nodes = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _nodes = world.ComponentMapper.GetMapper<Node>();
        }

        public void Process(ISimulationEvent<UserJoined> @event)
        {
            User? user = _scope.Peer!.Users.UpdateOrCreate(@event.Body.UserId, @event.Body.Claims);

            // Ensure the user has been added to the scope
            if (!_scope.Users.TryGet(user.Id, out _))
            {
                _scope.Users.Add(user);
            }

            ParallelKey key = @event.NewKey();
            if (@event.Simulation.HasEntity(key))
            { // This operation has already been done
                return;
            }

            Entity ship = _ships.CreateShip(ShipParts.HullSquare, @event);
            Entity pilot = _pilots.CreateUserPilot(user, ship, @event);


            Entity chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);
            chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);
            chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);

            return;
        }
    }
}
