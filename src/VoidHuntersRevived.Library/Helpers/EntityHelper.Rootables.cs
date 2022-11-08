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
            public static Entity CreateShip(World world, AetherBody body)
            {
                var entity = world.CreateEntity();

                EntityHelper.Rootables.MakeShip(entity, body);

                return entity;
            }

            public static void MakeShip(Entity entity, AetherBody body)
            {
                EntityHelper.Rootables.MakeRootable(entity, body);

                entity.Attach<Pilotable>(new Pilotable());
            }

            public static Entity CreateRootable(World world, AetherBody body)
            {
                var entity = world.CreateEntity();

                EntityHelper.Rootables.MakeRootable(entity, body);

                return entity;
            }

            public static void MakeRootable(Entity entity, AetherBody body)
            {
                entity.Attach<Rootable>(new Rootable());
                entity.Attach<AetherBody>(body);

                body.Tag = entity;
            }
        }
    }
}
