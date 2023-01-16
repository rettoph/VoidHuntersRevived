using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeTree(this Entity entity, Aether aether, Entity? head = null)
        {
            var tree = new Tree(entity);

            entity.Attach(aether.CreateBody(bodyType: BodyType.Dynamic));
            entity.Attach(tree);

            if(head is null)
            {
                return entity;
            }

            tree.Add(head);

            return entity;
        }

        public static Entity MakeChain(this Entity entity, Aether aether, Entity? head = null)
        {
            entity.MakeTree(aether, head);
            entity.Attach(Tractorable.Instance);

            return entity;
        }
    }
}
