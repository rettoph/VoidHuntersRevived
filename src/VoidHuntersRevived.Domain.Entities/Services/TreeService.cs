using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class TreeService : ITreeService
    {
        public Tree MakeTree(Entity entity, Body body, Node? head)
        {
            Tree tree = new Tree(entity.Id);
            body.Tag = entity.Id;

            entity.Attach(body);
            entity.Attach(tree);

            if (head is not null)
            {
                this.AddNode(head, tree);
            }

            return tree;
        }

        public void AddNode(Node node, Tree tree)
        {
            tree.Nodes.Add(node);
            node.Tree = tree;

            foreach (Degree degree in node.OutDegrees())
            {
                this.AddNode(degree.Node, tree);
            }
        }

        public void RemoveNode(Node node, Tree tree)
        {
            node.Tree = null;
            if (!tree.Nodes.Remove(node))
            {
                return;
            }

            foreach (Degree degree in node.OutDegrees())
            {
                this.RemoveNode(degree.Node, tree);
            }
        }
    }
}
