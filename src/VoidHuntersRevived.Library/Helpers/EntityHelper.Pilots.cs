using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Helpers
{
    public static partial class EntityHelper
    {
        public static class Pilots
        {
            public static Entity CreateUserPilot(
                World world, 
                User user)
            {
                return EntityHelper.Pilots.MakeUserPilot(
                    entity: world.CreateEntity(), 
                    user: user);
            }

            public static Entity MakeUserPilot(
                Entity entity, 
                User user)
            {
                entity.Attach(user);

                return EntityHelper.Pilots.MakePilot(entity);
            }

            public static Entity MakePilot(Entity entity)
            {
                entity.Attach(new Piloting());
                
                return entity;
            }
        }

    }
}
