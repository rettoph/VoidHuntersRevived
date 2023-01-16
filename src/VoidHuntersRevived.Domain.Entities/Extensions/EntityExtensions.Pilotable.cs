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
            entity.MakeTree(aether).MakePilotable();
            entity.Attach(new Ship(
                bridge: bridge,
                tree: entity.Get<Tree>()));

            return entity;
        }

        public static bool IsShip(this Entity entity)
        {
            return entity.Has<Ship>();
        }
    }
}
