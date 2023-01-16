using MonoGame.Extended.Entities;
using System.Collections.ObjectModel;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;

namespace VoidHuntersRevived.Common.Entities.ShipParts.Components
{
    public class Tree
    {
        private List<Node> _nodes;

        public readonly Entity Entity;
        public readonly ReadOnlyCollection<Node> Nodes;
        public Node? Head => _nodes.Any() ? _nodes[0] : null;

        public Tree(Entity entity)
        {
            _nodes = new List<Node>();

            this.Entity = entity;
            this.Nodes = new ReadOnlyCollection<Node>(_nodes);
        }

        public Node Add(Entity entity)
        {
            if (!entity.IsShipPart())
            {
                throw new ArgumentException($"{nameof(Tree)}::{nameof(Add)} - Argument '{nameof(entity)}' failed {nameof(Extensions.EntityExtensions.IsShipPart)} check.");
            }

            if (entity.Has<Node>())
            {
                var oldLeaf = entity.Get<Node>();

                oldLeaf.Tree.Remove(oldLeaf);
            }

            var node = new Node(entity, this);
            entity.Attach(node);
            _nodes.Add(node);

            return node;
        }

        public bool Remove(Node leaf)
        {
            if(!_nodes.Remove(leaf))
            {
                return false;
            }

            leaf.Entity.Detach<Node>();
            return true;
        }
    }
}
