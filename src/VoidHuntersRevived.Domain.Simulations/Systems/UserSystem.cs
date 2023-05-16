using Guppy.Attributes;
using Guppy.Network;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Domain.Entities;
using MonoGame.Extended.Entities;
using Microsoft.Xna.Framework;
using Guppy.Network.Identity;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserSystem : BasicSystem,
        ISimulationEventListener<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly ILogger _logger;

        public UserSystem(
            NetScope scope, 
            ILogger logger)
        {
            _scope = scope;
            _logger = logger;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);
        }

        public void Process(ISimulationEvent<UserJoined> @event)
        {
            Console.WriteLine("User Joined Event!");
            User? user = _scope.Peer!.Users.UpdateOrCreate(@event.Body.UserId, @event.Body.Claims);

            // Ensure the user has been added to the scope
            if (!_scope.Users.TryGet(user.Id, out _))
            {
                _scope.Users.Add(user);
            }

            ShipPart square = new ShipPart(
                joints: new[] { 
                    new Location(Vector2.Zero, 0) 
                }, 
                components: new ShipPartComponent[] {
                    Rigid.Polygon(1f, 4),
                    Drawable.Polygon(Colors.Orange, 4)
                });

            Entity ship = @event.Simulation.CreateShip(@event.NewKey(), square);
            ship.Attach(user);

            Entity shipPart = @event.Simulation.CreateShipPart(@event.NewKey(), square);

            return;
        }
    }
}
