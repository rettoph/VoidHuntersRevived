using Guppy.Attributes;
using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class TractoringSystem : ParallelEntityProcessingSystem,
        ISubscriber<IInput<StartTractoring>>,
        ISubscriber<IInput<StopTractoring>>
    {
        private static readonly AspectBuilder TractoringAspect = Aspect.All(new[] {
            typeof(Tractoring)
        });

        private readonly ITractorService _tractor;
        private readonly IBus _bus;

        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Parallelable> _parallelables;
        private INodeService _nodeService;
        private IChainService _chainService;

        public TractoringSystem(
            INodeService nodeService, 
            IChainService chainService, 
            ITractorService tractor, 
            IBus bus,
            ISimulationService simulations) : base(simulations, TractoringAspect)
        {
            _tractor = tractor;
            _nodeService = nodeService;
            _chainService = chainService;
            _bus = bus;

            _tractorings = default!;
            _tractorables = default!;
            _pilotables = default!;
            _pilotings = default!;
            _bodies = default!;
            _trees = default!;
            _nodes = default!;
            _parallelables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tractorings = mapperService.GetMapper<Tractoring>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _pilotables = mapperService.GetMapper<Pilotable>();
            _pilotings = mapperService.GetMapper<Piloting>();
            _bodies = mapperService.GetMapper<Body>();
            _trees = mapperService.GetMapper<Tree>();
            _nodes = mapperService.GetMapper<Node>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        public void Process(in IInput<StartTractoring> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Data.TargetTree, out int targetTreeId))
            {
                return;
            }

            if (!message.Simulation.TryGetEntityId(message.Data.TargetNode, out int targetNodeId))
            {
                return;
            }

            var pilotId = message.Simulation.GetEntityId(message.Sender);
            var piloting = _pilotings.Get(pilotId);
            var tree = _trees.Get(piloting.Pilotable.Id);

            if (!_nodes.TryGet(targetNodeId, out Node? targetNode))
            {
                return;
            }

            if(targetNode.Tree is null)
            {
                return;
            }

            if (targetNode.Tree.EntityId != targetTreeId)
            {
                // This almost always happens on a lockstep sent input within
                // The predictive simulation. This is because the node exists
                // but has been attached to another tree after the predictive
                // simulation pre-detached a part from the current ship, and
                // created a brand new chain in the process. Ideally there would
                // be some sort of prediction verification done before we got to
                // this state?
                return;
            }

            if (!_tractorables.TryGet(targetTreeId, out var tractorable))
            { // The target is not tractorable
                return;
            }


            if (tractorable.WhitelistedTractoring is not null && tractorable.WhitelistedTractoring.Value != piloting.Pilotable.Id)
            { // This part is attached to another ship
                return;
            }

            if (targetNode.Tree == tree)
            { // The selected node is attached to the current ship

                // Cache all the values we're about to delete...
                var key = _parallelables.Get(targetNode.EntityId).Key;
                var position = targetNode.WorldPosition;
                var rotation = targetNode.WorldTransformation.Radians();

                // Destroy the link to the ship's tree
                _nodeService.Detach(targetNode.ChildJoint()?.Link ?? throw new NotImplementedException());

                // Create a brand new chain to hold the detached parts.
                // Notice we've updated the tractorableId to the new chain id
                // This is why the comment above happens: A different tractorableId
                // was slotted in already, but the confirmation from the server doesn't
                // display that very well.
                targetTreeId = _chainService.CreateChain(
                    key: key.Create(ParallelTypes.Chain),
                    head: targetNode,
                    position: position,
                    rotation: rotation,
                    simulation: message.Simulation).Id;
            }

            var tractoring = new Tractoring(piloting.Pilotable.Id, targetTreeId);
            _tractorings.Put(piloting.Pilotable.Id, tractoring);
        }

        public void Process(in IInput<StopTractoring> message)
        {
            int pilotId = message.Simulation.GetEntityId(message.Sender);
            if(!_pilotings.TryGet(pilotId, out Piloting? piloting))
            {
                return;
            }
            
            if(!message.Simulation.TryGetEntityId(message.Data.TargetTreeKey, out int tractorableId))
            {
                return;
            }
            
            if(_tractorings.TryGet(piloting.Pilotable.Id, out Tractoring? tractoring)
                && tractoring.TargetTreeId == tractorableId)
            {
                _tractorings.Delete(piloting.Pilotable.Id);
            }
            
            if(!_tractor.TransformTractorable(message.Data.TargetPosition, piloting.Pilotable.Id, tractorableId, out Link? potential))
            {
                return;
            }

            // Destroy the old chain
            if(potential.Child.Node.Tree is not null && _parallelables.TryGet(potential.Child.Node.Tree.EntityId, out Parallelable? chain))
            {
                message.Simulation.DestroyEntity(chain.Key);
            }

            // Create a jointing to the current ship.
            _nodeService.Attach(potential.Child, potential.Parent);
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            var tractoring = _tractorings.Get(entityId);

            _tractor.TransformTractorable(pilotable.Aim.Value, tractoring, out _);
        }
    }
}
