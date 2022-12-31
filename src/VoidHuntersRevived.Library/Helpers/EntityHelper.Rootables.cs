using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;

namespace VoidHuntersRevived.Library.Helpers
{
    public static partial class EntityHelper
    {
        public static class Rootables
        {
            public static Entity MakeShip(Entity entity, AetherBody body)
            {
                entity.Attach<Pilotable>(new Pilotable());

                return EntityHelper.Rootables.MakeRootable(entity, body);
            }

            public static Entity MakeRootable(Entity entity, AetherBody body)
            {
                entity.Attach<Rootable>(new Rootable());
                entity.Attach<AetherBody>(body);

                body.Tag = entity;

                return entity;
            }
        }
    }
}
