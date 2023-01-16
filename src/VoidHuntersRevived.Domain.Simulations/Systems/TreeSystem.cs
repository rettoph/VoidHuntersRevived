using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class TreeSystem : ParallelEntityProcessingSystem,
        ISubscriber<IEvent<CleanJointed>>
    {
        private static readonly AspectBuilder TreeAspect = Aspect.All(new[]
        {
            typeof(Tree)
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

            var worldTransformation = body.GetWorldMatrix();

            foreach(var node in tree.Nodes)
            {
                node.WorldTransformation = node.LocalTransformation * worldTransformation;
            }
        }

        public void Process(in IEvent<CleanJointed> message)
        {
            // The entity is currently attached to another tree
            if (_nodes.TryGet(message.Data.Jointed.Joint.Jointable.Entity.Id, out var oldNode))
            { // Detach it from its old tree
                oldNode.Tree.Remove(oldNode);
            }

            var tree = _nodes.Get(message.Data.Jointed.Parent.Jointable.Entity.Id).Tree;
            tree.Add(
                entity: message.Data.Jointed.Joint.Jointable.Entity, 
                localTransformation: message.Data.Jointed.LocalTransformation);
        }
    }
}
