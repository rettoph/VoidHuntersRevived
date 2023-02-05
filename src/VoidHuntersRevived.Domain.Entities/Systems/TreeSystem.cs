using Guppy.Attributes;
using Guppy.Common;
using Guppy.Common.Attributes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    [Sortable<ISubscriber<IEvent<CreateNode>>>(int.MinValue)]
    [Sortable<ISubscriber<IEvent<CreateJointing>>>(int.MinValue + 10)]
    internal sealed class TreeSystem : ParallelEntityProcessingSystem,
        ISubscriber<IEvent<CreateNode>>,
        ISubscriber<IEvent<DestroyNode>>,
        ISubscriber<IEvent<CreateJointing>>
    {
        private static readonly AspectBuilder TreeAspect = Aspect.All(new[]
        {
            typeof(Tree),
            typeof(Body)
        });

        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Jointing> _jointings;
        private ComponentMapper<Jointee> _jointees;
        private ComponentMapper<Jointable> _jointables;
        private ComponentMapper<Body> _bodies;

        public TreeSystem(ISimulationService simulations) : base(simulations, TreeAspect)
        {
            _trees = default!;
            _nodes = default!;
            _jointings = default!;
            _jointees = default!;
            _jointables = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trees = mapperService.GetMapper<Tree>();
            _nodes = mapperService.GetMapper<Node>();
            _jointings = mapperService.GetMapper<Jointing>();
            _jointees = mapperService.GetMapper<Jointee>();
            _jointables = mapperService.GetMapper<Jointable>();
            _bodies = mapperService.GetMapper<Body>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var tree = _trees.Get(entityId);
            var body = _bodies.Get(entityId);

            var worldTransformation = body.GetTransformation();

            foreach(var nodeId in tree.Nodes)
            {
                var node = _nodes.Get(nodeId);
                node.WorldTransformation = node.LocalTransformation * worldTransformation;
            }
        }

        public void Process(in IEvent<CreateNode> message)
        {
            // Check to see if the entity is already attached to another
            // tree. If it is, we should remove the node first.
            if (_nodes.TryGet(message.Data.NodeId, out var existingNode))
            { // Detach it from its old tree
                message.Simulation.PublishEvent(new DestroyNode()
                {
                    NodeId = message.Data.NodeId,
                    TreeId = existingNode.TreeId,
                });
            }

            var tree = _trees.Get(message.Data.TreeId);
            var jointing = _jointings.Get(message.Data.NodeId);
            var jointable = _jointables.Get(message.Data.NodeId);
            var node = new Node(
                entityId: message.Data.NodeId,
                treeId: message.Data.TreeId,
                center: jointable.Configuration.LocalCenter,
                localTransformation: jointing is null ? Matrix.Identity : jointing.LocalTransformation);

            tree.Add(node);
            _nodes.Put(node.EntityId, node);

            if (!_jointees.TryGet(node.EntityId, out var jointee))
            {
                return;
            }

            // Recersively remove all children from the tree as well.
            foreach (var child in jointee.Children)
            {
                message.Simulation.PublishEvent(new CreateNode()
                {
                    NodeId = child.Joint.Entity.Id,
                    TreeId = message.Data.TreeId,
                });
            }
        }

        public void Process(in IEvent<DestroyNode> message)
        {
            var node = _nodes.Get(message.Data.NodeId);
            var tree = _trees.Get(message.Data.TreeId);

            if (!tree.Remove(node))
            {
                return;
            }

            _nodes.Delete(node.EntityId);

            if(!_jointees.TryGet(node.EntityId, out var jointee))
            {
                return;
            }

            // Recersively remove all children from the tree as well.
            foreach(var child in jointee.Children)
            {
                message.Simulation.PublishEvent(new DestroyNode()
                {
                    NodeId = child.Joint.Entity.Id,
                    TreeId = message.Data.TreeId,
                });
            }
        }

        public void Process(in IEvent<CreateJointing> message)
        {
            var nodeId = message.Simulation.GetEntityId(message.Data.Joint);
            var parentId = message.Simulation.GetEntityId(message.Data.Parent);
            var parentTree = _nodes.Get(parentId).TreeId;

            // Publish an AddNode event whenever a new joint is created.
            message.Simulation.PublishEvent(new CreateNode()
            {
                NodeId = nodeId,
                TreeId = parentTree
            });
        }
    }
}
