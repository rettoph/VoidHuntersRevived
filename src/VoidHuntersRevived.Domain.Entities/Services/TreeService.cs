using Guppy.Common;
using Guppy.Common.Collections;
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
        private readonly IBus _bus;
        private readonly Queue<Node> _buffer;

        public TreeService(IBus bus)
        {
            _bus = bus;
            _buffer = new Queue<Node>();
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

            _buffer.Enqueue(node);
            while(_buffer.TryDequeue(out Node? dirty))
            {
                tree.Nodes.Add(dirty);
                dirty.Tree = tree;

                foreach (Joint child in dirty.ParentJoints())
                {
                    _buffer.Enqueue(child.Link!.Child.Node);
                }

                _bus.Publish(new Added<Node, Tree>(dirty, tree));
            }
        }

        public void RemoveNode(Node node, Tree tree)
        {
            if (node.Tree != tree)
            {
                return;
            }

            _buffer.Enqueue(node);
            while (_buffer.TryDequeue(out Node? dirty))
            {
                tree.Nodes.Remove(dirty);
                dirty.Tree = null;

                foreach (Joint child in dirty.ParentJoints())
                {
                    _buffer.Enqueue(child.Link!.Child.Node);
                }

                _bus.Publish(new Removed<Node, Tree>(dirty, tree));
            }
        }
    }
}
