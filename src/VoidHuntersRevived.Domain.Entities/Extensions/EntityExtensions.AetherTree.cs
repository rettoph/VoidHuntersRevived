using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Entities.Components;

namespace VoidHuntersRevived.Domain.Entities.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeAetherTree(this Entity entity, Aether aether)
        {
            entity.Attach(aether.CreateBody(bodyType: BodyType.Dynamic));
            entity.Attach(new AetherTree(entity));

            return entity;
        }

        public static bool IsAetherTree(this Entity entity)
        {
            return entity.Has<Body>() && entity.Has<AetherTree>();
        }
    }
}
