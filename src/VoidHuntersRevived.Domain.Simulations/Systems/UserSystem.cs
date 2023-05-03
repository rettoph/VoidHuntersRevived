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
using VoidHuntersRevived.Common.Simulations.Enums;
using VoidHuntersRevived.Common.Entities.Services;
using Guppy.Network.Identity;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public SimulationEventResult Process(ISimulationEvent<UserJoined> @event)
        {
            User? user = _scope.Peer!.Users.UpdateOrCreate(@event.Body.UserId, @event.Body.Claims);
            ParallelKeyGenerator keys = new ParallelKeyGenerator(@event.Key);

            // Ensure the user has been added to the scope
            if (!_scope.Users.TryGet(user.Id, out _))
            {
                _scope.Users.Add(user);
            }

            ParallelKey key = keys.Next();
            if (@event.Simulation.HasEntity(key))
            { // This operation has already been done
                return SimulationEventResult.Failure;
            }

            Entity ship = _ships.CreateShip(ShipParts.HullSquare, @event);
            Entity pilot = _pilots.CreateUserPilot(user, ship, @event);


            Entity chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);
            chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);
            chain = _chains.CreateChain(ShipParts.HullSquare, Vector2.Zero, 0, @event);

            return SimulationEventResult.Success;
        }
    }
}
