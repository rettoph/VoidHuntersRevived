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
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
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

        public Entity CreateShip(ParallelKey key, string bridgeResource, ISimulation simulation)
        {
            Entity bridge = _shipParts.CreateShipPart(key.Create(ParallelTypes.ShipPart), simulation, bridgeResource);
            Entity ship = simulation.CreateEntity(key);
            this.MakeShip(ship, bridge, simulation);

            return ship;
        }

        public void MakeShip(Entity entity, Entity bridge, ISimulation simulation)
        {
            Body body = simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            Node node = bridge.Get<Node>();

            _trees.MakeTree(entity, body, node);

            entity.Attach(new Ship(
                entityId: entity.Id,
                bridge: bridge));

            entity.Attach(new Tractorable(entity.Id));

            entity.MakePilotable();
        }
    }
}
