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
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Messages;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    [Sortable<ISubscriber<Created<Link>>>(int.MinValue)]
    [Sortable<ISubscriber<Destroyed<Link>>>(int.MinValue + 10)]
    internal sealed class TreeSystem : ParallelEntityProcessingSystem,
        ISubscriber<Created<Link>>,
        ISubscriber<Destroyed<Link>>
    {
        private static readonly AspectBuilder TreeAspect = Aspect.All(new[]
        {
            typeof(Tree),
            typeof(Body)
        });

        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Body> _bodies;
        private readonly ITreeService _treeService;

        public TreeSystem(ITreeService treeService, ISimulationService simulations) : base(simulations, TreeAspect)
        {
            _trees = default!;
            _nodes = default!;
            _bodies = default!;
            _treeService = treeService;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _trees = mapperService.GetMapper<Tree>();
            _nodes = mapperService.GetMapper<Node>();
            _bodies = mapperService.GetMapper<Body>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var tree = _trees.Get(entityId);
            var body = _bodies.Get(entityId);

            var worldTransformation = body.GetTransformation();

            foreach(var node in tree.Nodes)
            {
                node.WorldTransformation = node.LocalTransformation * worldTransformation;
            }
        }

        public void Process(in Created<Link> message)
        {
            Tree? tree = message.Instance.Parent.Node.Tree;
            Node node = message.Instance.Child.Node;

            if(tree is not null)
            {
                _treeService.AddNode(node, tree);
            }
        }

        public void Process(in Destroyed<Link> message)
        {
            Tree? tree = message.Instance.Parent.Node.Tree;
            Node node = message.Instance.Child.Node;

            if (tree is not null)
            {
                _treeService.RemoveNode(node, tree);
            }
        }
    }
}
