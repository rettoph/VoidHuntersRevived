using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeShipPart(this Entity entity, ShipPartResource configuration)
        {
            configuration.Make(entity);
            entity.Attach(configuration);
            entity.Attach(new Jointee(entity));

            return entity;
        }

        public static bool IsShipPart(this Entity entity)
        {
            return entity.Has<ShipPartResource>();
        }
    }
}
