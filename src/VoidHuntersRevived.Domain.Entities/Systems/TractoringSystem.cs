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
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Entities.ShipParts.Extensions;
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
        ISubscriber<IEvent<StartTractoring>>,
        ISubscriber<IEvent<StopTractoring>>
    {
        private static readonly AspectBuilder TractoringAspect = Aspect.All(new[] {
            typeof(Tractoring)
        });

        private readonly ITractorService _tractor;

        private ComponentMapper<Tractoring> _tractorings;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Piloting> _pilotings;
        private ComponentMapper<Body> _bodies;
        private ComponentMapper<Tree> _trees;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Jointing> _jointed;
        private ComponentMapper<Parallelable> _parallelables;

        public TractoringSystem(ITractorService tractor, ISimulationService simulations) : base(simulations, TractoringAspect)
        {
            _tractor = tractor;

            _tractorings = default!;
            _tractorables = default!;
            _pilotables = default!;
            _pilotings = default!;
            _bodies = default!;
            _trees = default!;
            _nodes = default!;
            _jointed = default!;
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
            _jointed = mapperService.GetMapper<Jointing>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        public void Process(in IEvent<StartTractoring> message)
        {
            if (!message.Target.TryGetEntityId(message.Data.Tractorable, out int tractorableId))
            {
                return;
            }

            if (!message.Target.TryGetEntityId(message.Data.Node, out int nodeId))
            {
                return;
            }

            var pilotId = message.Target.GetEntityId(message.Sender);
            var piloting = _pilotings.Get(pilotId);
            var tree = _trees.Get(piloting.Pilotable.Id);

            if (!_nodes.TryGet(nodeId, out var node))
            {
                return;
            }

            if (node.TreeId != tractorableId)
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

            if (!_tractorables.TryGet(tractorableId, out var tractorable))
            { // The target is not tractorable
                return;
            }


            if (tractorable.WhitelistedTractoring is not null && tractorable.WhitelistedTractoring.Value != piloting.Pilotable.Id)
            { // This part is attached to another ship
                return;
            }

            if (node.TreeId == tree.EntityId && _jointed.Has(node.EntityId))
            { // The selected node is attached to the current ship

                // Cache all the values we're about to delete...
                var key = _parallelables.Get(node.EntityId).Key;
                var position = node.WorldPosition;
                var rotation = node.WorldTransformation.Radians();

                // Destroy the joint to the ship
                message.PublishConsequent(new DestroyJointing()
                {
                    Jointed = key
                });

                // Create a brand new chain to hold the detached parts.
                // Notice we've updated the tractorableId to the new chain id
                // This is why the comment above happens: A different tractorableId
                // was slotted in already, but the confirmation from the server doesn't
                // display that very well.
                tractorableId = message.Target.CreateChain(
                    @event: message,
                    key: key.Create(ParallelTypes.Chain), 
                    headId: node.EntityId,
                    position: position,
                    rotation: rotation).Id;
            }

            var tractoring = new Tractoring(piloting.Pilotable.Id, tractorableId);
            _tractorings.Put(piloting.Pilotable.Id, tractoring);
        }

        public void Process(in IEvent<StopTractoring> message)
        {
            int pilotId = message.Target.GetEntityId(message.Sender);
            if(!_pilotings.TryGet(pilotId, out Piloting? piloting))
            {
                return;
            }

            if(!message.Target.TryGetEntityId(message.Data.TractorableKey, out int tractorableId))
            {
                return;
            }

            if(_tractorings.TryGet(piloting.Pilotable.Id, out Tractoring? tractoring)
                && tractoring.TractorableId == tractorableId)
            {
                _tractorings.Delete(piloting.Pilotable.Id);
            }

            if(!_tractor.TransformTractorable(message.Data.TargetPosition, piloting.Pilotable.Id, tractorableId, out Jointing? potential))
            {
                return;
            }

            // Destroy the old chain
            message.PublishConsequent(new DestroyEntity()
            {
                Key = _parallelables.Get(potential.Joint.Entity.Get<Node>().TreeId).Key
            });

            // Create a jointing to the current ship.
            message.PublishConsequent(new CreateJointing()
            {
                Parent = potential.Parent.Entity.Get<Parallelable>().Key,
                ParentJointId = potential.Parent.Index,
                Joint = potential.Joint.Entity.Get<Parallelable>().Key,
                JointId = potential.Joint.Index
            });
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            var tractoring = _tractorings.Get(entityId);

            _tractor.TransformTractorable(pilotable.Aim.Value, tractoring, out _);
        }
    }
}
