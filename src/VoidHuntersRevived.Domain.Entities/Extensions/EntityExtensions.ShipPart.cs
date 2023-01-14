using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeShipPart(this Entity entity, ShipPartConfiguration configuration)
        {
            configuration.Make(entity);
            entity.Attach(configuration);
            entity.Attach(new Linking(entity));

            return entity;
        }

        public static bool IsShipPart(this Entity entity)
        {
            return entity.Has<ShipPartConfiguration>();
        }
    }
}
