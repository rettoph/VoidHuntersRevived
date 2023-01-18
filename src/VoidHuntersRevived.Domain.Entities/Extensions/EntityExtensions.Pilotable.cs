using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using tainicom.Aether.Physics2D.Dynamics;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakePilotable(this Entity entity)
        {
            entity.Attach(new Pilotable());

            return entity;
        }

        public static bool IsPilotable(this Entity entity)
        {
            return entity.Has<Pilotable>();
        }

        public static Entity MakeShip(this Entity entity, Aether aether, Entity bridge)
        {
            entity.MakeTree(
                body: aether.CreateBody(bodyType: BodyType.Dynamic), 
                head: bridge).MakePilotable();

            entity.Attach(new Ship(
                bridge: bridge,
                tree: entity));

            return entity;
        }

        public static bool IsShip(this Entity entity)
        {
            return entity.Has<Ship>();
        }
    }
}
