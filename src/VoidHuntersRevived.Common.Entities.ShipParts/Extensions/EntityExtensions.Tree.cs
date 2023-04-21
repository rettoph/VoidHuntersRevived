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
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Extensions;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Extensions
{
    public static partial class EntityExtensions
    {
        public static Entity MakeTree(this Entity entity, IEvent @event, Body body, int? headId = null)
        {
            var tree = new Tree(entity.Id);
            body.Tag = entity.Id;

            entity.Attach(body);
            entity.Attach(tree);

            if(headId is not null)
            {
                @event.PublishConsequent(new CreateNode()
                {
                    NodeId = headId.Value,
                    TreeId = tree.EntityId,
                });
            }

            return entity;
        }

        public static Entity MakeChain(this Entity entity, IEvent @event, Aether aether, int? headId = null, Vector2 position = default, float rotation = 0)
        {
            var body = aether.CreateBody(bodyType: BodyType.Dynamic);
            body.SetTransformIgnoreContacts(ref position, rotation);
            body.OnCollision += HandleChainCollision;

            entity.MakeTree(@event, body, headId);
            entity.Attach(Tractorable.Instance);

            return entity;
        }

        private static bool HandleChainCollision(Fixture sender, Fixture other, Contact contact)
        {
            return false;
        }
    }
}
