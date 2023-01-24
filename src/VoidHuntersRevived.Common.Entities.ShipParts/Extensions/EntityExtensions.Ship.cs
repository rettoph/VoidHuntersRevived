using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Common.Entities.Extensions
{
    public static partial class EntityExtensions
    {
        public static bool IsPilotable(this Entity entity)
        {
            return entity.Has<Pilotable>();
        }

        public static Entity MakeShip(this Entity entity, Aether aether, Entity bridge)
        {
            entity.MakeTree(
                body: aether.CreateBody(bodyType: BodyType.Dynamic), 
                headId: bridge.Id).MakePilotable();

            entity.Attach(new Ship(
                entityId: entity.Id,
                bridge: bridge));

            entity.Attach(new Tractorable(entity.Id));

            return entity;
        }

        public static bool IsShip(this Entity entity)
        {
            return entity.Has<Ship>();
        }
    }
}
