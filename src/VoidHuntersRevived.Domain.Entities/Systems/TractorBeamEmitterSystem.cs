using Guppy.Attributes;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Collision;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Services;
using VoidHuntersRevived.Common.Entities.Tractoring;
using VoidHuntersRevived.Common.Entities.Tractoring.Components;
using VoidHuntersRevived.Common.Entities.Tractoring.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    [GuppyFilter<IGameGuppy>]
    internal sealed class TractorBeamEmitterSystem : ParallelEntityProcessingSystem,
        ISimulationEventListener<TryStartTractoring>,
        ISimulationEventListener<TryStopTractoring>,
        ISimulationEventListener<StartTractoring>,
        ISimulationEventListener<StopTractoring>
    {
        private static readonly AspectBuilder TractorBeamEmitterAspect = Aspect.All(new[]
        {
            typeof(TractorBeamEmitter)
        });

        private ITractorBeamService _tractorBeams;
        private INodeService _nodeService;
        private ComponentMapper<TractorBeamEmitter> _emitters;
        private ComponentMapper<Node> _nodes;
        private ComponentMapper<Tractorable> _tractorables;
        private ComponentMapper<Parallelable> _parallelables;

        public TractorBeamEmitterSystem(
            ISimulationService simulations, 
            ITractorBeamService tractorBeams,
            INodeService nodeService) : base(simulations, TractorBeamEmitterAspect)
        {
            _tractorBeams = tractorBeams;
            _nodeService = nodeService;
            _emitters = default!;
            _nodes = default!;
            _tractorables = default!;
            _parallelables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _emitters = mapperService.GetMapper<TractorBeamEmitter>();
            _nodes = mapperService.GetMapper<Node>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            _tractorBeams.Update(entityId);
        }

        public void Process(ISimulationEvent<TryStartTractoring> @event)
        {
            if(!@event.Simulation.TryGetEntityId(@event.Body.EmitterKey, out int emitterId))
            {
                return;
            }

            if(!_tractorBeams.Query(@event.Simulation, emitterId, out int targetId))
            {
                return;
            }

            if(!_parallelables.TryGet(targetId, out Parallelable? parallelable))
            {
                return;
            }

            @event.Simulation.Publish(new SimulationEventData()
            {
                Key = @event.NewKey().Merge(parallelable.Key).Merge(@event.Body.EmitterKey),
                SenderId = @event.SenderId,
                Body = new StartTractoring()
                {
                    EmitterKey = @event.Body.EmitterKey,
                    TargetKey = parallelable.Key
                }
            });
        }

        public void Process(ISimulationEvent<TryStopTractoring> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.EmitterKey, out int emitterId))
            {
                return;
            }

            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                return;
            }

            if(emitter.TractorBeam is null)
            {
                return;
            }

            if (!_parallelables.TryGet(emitter.TractorBeam.Target.EntityId, out Parallelable? parallelable))
            {
                return;
            }

            @event.Simulation.Publish(new SimulationEventData()
            {
                Key = @event.NewKey().Merge(parallelable.Key).Merge(@event.Body.EmitterKey),
                SenderId = @event.SenderId,
                Body = new StopTractoring()
                {
                    EmitterKey = @event.Body.EmitterKey
                }
            });
        }

        public void Process(ISimulationEvent<StartTractoring> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.EmitterKey, out int emitterId))
            {
                return;
            }

            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                return;
            }

            if (!@event.Simulation.TryGetEntityId(@event.Body.TargetKey, out int targetId))
            {
                return;
            }

            if (!_nodes.TryGet(targetId, out Node? target))
            {
                return;
            }

            if(target.Tree is null)
            {
                return;
            }

            if(!_tractorables.TryGet(target.Tree.EntityId, out Tractorable? tractorable))
            {
                // Detach the node from its current tree and create a brand new chain.
                throw new NotImplementedException();
            }

            emitter.TractorBeam = new TractorBeam(tractorable);
        }

        public void Process(ISimulationEvent<StopTractoring> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.EmitterKey, out int emitterId))
            {
                return;
            }

            if (!_emitters.TryGet(emitterId, out TractorBeamEmitter? emitter))
            {
                return;
            }

            if(emitter.TractorBeam is null)
            {
                return;
            }

            if (_tractorBeams.TryGetPotentialLink(emitterId, out Link? link))
            {
                _nodeService.Attach(link.Child, link.Parent);
            }

            emitter.TractorBeam = null;
        }
    }
}
