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
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [Sortable<ISubscriber<IEvent<CleanJointed>>>(int.MinValue + 10)]
    internal sealed class TreeSystem : ParallelEntityProcessingSystem,
        ISubscriber<IEvent<CreateTree>>,
        ISubscriber<IEvent<CleanJointed>>
    {
        private static readonly AspectBuilder TreeAspect = Aspect.All(new[]
        {
            typeof(Tree),
            typeof(Body)
        });

        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Jointed> _jointed;
        private ComponentMapper<Body> _bodies;

        public TreeSystem(ISimulationService simulations) : base(simulations, TreeAspect)
        {
            _trees = default!;
            _nodes = default!;
            _jointed = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trees = mapperService.GetMapper<Tree>();
            _nodes = mapperService.GetMapper<Node>();
            _jointed = mapperService.GetMapper<Jointed>();
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

        public void Process(in IEvent<CleanJointed> message)
        {
            // The entity is currently attached to another tree
            if (_nodes.TryGet(message.Data.Jointed.Joint.Entity, out var oldNode))
            { // Detach it from its old tree
                var oldTree = _trees.Get(oldNode.Tree);
                oldTree.Remove(oldNode.Entity);
            }

            if(message.Data.Status == CleanJointed.Statuses.Create)
            {
                var node = _nodes.Get(message.Data.Jointed.Parent.Entity);
                var tree = _trees.Get(node.Tree);

                tree.Add(
                    entity: message.Data.Jointed.Joint.Entity,
                    localTransformation: message.Data.Jointed.LocalTransformation);
            }
        }

        public void Process(in IEvent<CreateTree> message)
        {
            var entity = message.Simulation.CreateEntity(message.Data.Key);
            var body = message.Data.Body;
            var head = message.Data.Head;

            var tree = new Tree(entity);
            body.Tag = entity.Id;
            body.SetTransformIgnoreContacts(message.Data.Position, message.Data.Rotation);

            entity.Attach(body);
            entity.Attach(tree);

            if (head is not null)
            {
                tree.Add(head, Matrix.Identity);
            }
        }
    }
}
