using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Tree
    {
        private readonly List<Entity> _nodes;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Entity> Nodes;
        public Entity? Head => _nodes.Any() ? _nodes[0] : null;

        public Tree(Entity entity)
        {
            _nodes = new List<Entity>();

            this.Entity = entity;
            this.Nodes = new ReadOnlyCollection<Entity>(_nodes);
        }

        public Node Add(Entity entity, Matrix localTransformation)
        {
            if (entity.Has<Node>())
            {
                throw new NotImplementedException();
            }

            var node = new Node(entity, this.Entity, localTransformation);
            entity.Attach(node);
            _nodes.Add(entity);

            return node;
        }

        public bool Remove(Entity entity)
        {
            if(_nodes.Remove(entity))
            {
                entity.Detach<Node>();
                return true;
            }

            return false;
        }
    }
}
