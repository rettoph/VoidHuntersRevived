using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeTree(this Entity entity, Body body, Entity? head = null)
        {
            var tree = new Tree(entity);
            body.Tag = entity.Id;

            entity.Attach(body);
            entity.Attach(tree);

            if(head is null)
            {
                return entity;
            }

            tree.Add(head, Matrix.Identity);

            return entity;
        }

        public static Entity MakeChain(this Entity entity, Aether aether, Entity? head = null, Vector2 position = default, float rotation = 0)
        {
            var body = aether.CreateBody(bodyType: BodyType.Dynamic);
            body.SetTransformIgnoreContacts(ref position, rotation);
            body.OnCollision += HandleChainCollision;

            entity.MakeTree(body, head);
            entity.Attach(Tractorable.Instance);

            return entity;
        }

        private static bool HandleChainCollision(Fixture sender, Fixture other, Contact contact)
        {
            return false;
        }
    }
}
