﻿using Guppy.Common;
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
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class TreeService : ITreeService
    {
        public readonly IBus _bus;

        public TreeService(IBus bus)
        {
            _bus = bus;
        }

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
            if(node.Tree == tree)
            {
                return;
            }

            if(node.Tree is not null)
            {
                this.RemoveNode(node, node.Tree);
            }

            tree.Nodes.Add(node);
            node.Tree = tree;
            _bus.Publish(new Added<Node, Tree>(node, tree));

            foreach (Degree degree in node.OutDegrees())
            {
                this.AddNode(degree.Node, tree);
            }
        }

        public void RemoveNode(Node node, Tree tree)
        {
            
            if (!tree.Nodes.Remove(node))
            {
                return;
            }

            node.Tree = null;
            _bus.Publish(new Removed<Node, Tree>(node, tree));

            foreach (Degree degree in node.OutDegrees())
            {
                this.RemoveNode(degree.Node, tree);
            }
        }
    }
}
