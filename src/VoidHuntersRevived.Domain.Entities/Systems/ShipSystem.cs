using Guppy.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class ShipSystem : BasicSystem,
        ISubscriber<IEvent<CreateShip>>
    {
        public void Process(in IEvent<CreateShip> message)
        {
            // Setup pre-requisites
            var body = message.Simulation.Aether.CreateBody(bodyType: BodyType.Dynamic);
            var bridge = message.Simulation.CreateShipPart(
                key: message.Data.Key.Create(ParallelTypes.ShipPart),
                configuration: message.Data.BridgeConfiguration);

            // Invoke Pre-requisites
            message.Simulation.PublishEvent(new CreateTree(
                key: message.Data.Key,
                body: body,
                head: bridge,
                position: Vector2.Zero,
                rotation: 0));

            // Setup entity
            var entity = message.Simulation.GetEntity(message.Data.Key);

            entity.Attach(new Ship(
                entityId: entity.Id,
                bridge: bridge));

            entity.Attach(new Tractorable(entity.Id));
            entity.Attach(new Pilotable(entity.Id));
        }
    }
}
