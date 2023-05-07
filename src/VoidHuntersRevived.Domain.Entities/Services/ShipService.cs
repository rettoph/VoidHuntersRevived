using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class ShipService : IShipService
    {
        private readonly IShipPartService _shipParts;
        private readonly ITreeService _trees;

        public ShipService(IShipPartService shipParts, ITreeService trees)
        {
            _shipParts = shipParts;
            _trees = trees;
        }

        public Entity CreateShip(string bridgeResource, ISimulationEvent @event)
        {
            Entity bridge = _shipParts.CreateShipPart(bridgeResource, @event);
            Entity ship = @event.Simulation.CreateEntity(@event.NewKey());
            this.MakeShip(ship, bridge, @event);

            return ship;
        }

        public void MakeShip(Entity entity, Entity bridge, ISimulationEvent @event)
        {
            Body body = @event.Simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            Node node = bridge.Get<Node>();

            _trees.MakeTree(entity, body, node);

            entity.Attach(new Ship(
                entityId: entity.Id,
                bridge: bridge));

            entity.Attach(new TractorBeamEmitter(entity.Id));

            entity.MakePilotable();
        }
    }
}
