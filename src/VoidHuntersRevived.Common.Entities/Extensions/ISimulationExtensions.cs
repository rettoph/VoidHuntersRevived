using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static class ISimulationExtensions
    {
        public static Entity CreateShip(this ISimulation simulation, ParallelKey key, ShipPart bridge)
        {
            return simulation.CreateEntity(key, EntityTypes.Ship, entity =>
            {
                Body body = simulation.Aether.CreateBody(Vector2.Zero, 0, BodyType.Dynamic);
                WorldLocation worldLocation = new AetherBodyWorldLocation(body);

                entity.Attach(body);
                entity.Attach(worldLocation);
                bridge.Clone().AttachTo(entity);

                body.Tag = entity.Id;
            });
        }

        public static Entity CreateShipPart(this ISimulation simulation, ParallelKey key, ShipPart shipPart)
        {
            return simulation.CreateEntity(key, EntityTypes.TractorableShipPart, entity =>
            {
                Body body = simulation.Aether.CreateBody(Vector2.Zero, 0, BodyType.Dynamic);
                WorldLocation worldLocation = new AetherBodyWorldLocation(body);

                entity.Attach(body);
                entity.Attach(worldLocation);
                shipPart.Clone().AttachTo(entity);

                body.Tag = entity.Id;
            });
        }
    }
}
