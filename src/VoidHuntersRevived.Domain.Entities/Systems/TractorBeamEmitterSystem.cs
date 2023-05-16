using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using tainicom.Aether.Physics2D.Collision;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.Entities.ShipParts.Events;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class TractorBeamEmitterSystem : ParallelEntityProcessingSystem,
        ISimulationEventListener<ActivateTractorBeamEmitter>,
        ISimulationEventListener<CreateTractorBeamEmitterTarget>,
        ISimulationEventListener<DeactivateTractorBeamEmitter>
    {
        private static readonly AspectBuilder TractorBeamEmitterAspect = Aspect.All(new[]
        {
            typeof(TractorBeamEmitter),
            typeof(Tactical)
        });

        private ComponentMapper<TractorBeamEmitter> _tractorBeamEmitters = null!;
        private ComponentMapper<Tactical> _tacticals = null!;
        private ComponentMapper<Tractorable> _tractorables = null!;
        private ComponentMapper<ShipPart> _shipParts = null!;
        private ComponentMapper<WorldLocation> _worldLocations = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public TractorBeamEmitterSystem(ISimulationService simulations) : base(simulations, TractorBeamEmitterAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tractorBeamEmitters = mapperService.GetMapper<TractorBeamEmitter>();
            _tacticals = mapperService.GetMapper<Tactical>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _shipParts = mapperService.GetMapper<ShipPart>();
            _worldLocations = mapperService.GetMapper<WorldLocation>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            if (!_tractorBeamEmitters.TryGet(entityId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetId is null)
            {
                return;
            }

            if (!_tacticals.TryGet(entityId, out Tactical? tactical))
            {
                return;
            }

            _worldLocations.Get(tractorBeamEmitter.TargetId.Value).SetTransform(tactical.Value, 0);
        }

        public void Process(ISimulationEvent<ActivateTractorBeamEmitter> @event)
        {
            if(!@event.Simulation.TryGetEntityId(@event.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetId is not null)
            {
                return;
            }

            if (!_tacticals.TryGet(tractorBeamEmitterId, out Tactical? tactical))
            {
                return;
            }

            if (!this.Query(@event.Simulation, tractorBeamEmitter, tactical.Value, out int targetId))
            { // No valid potential targets found...
                return;
            }

            Parallelable parallelable = _parallelables.Get(targetId);

            @event.Simulation.Publish(new SimulationEventData()
            {
                Key = @event.NewKey(),
                SenderId = @event.SenderId,
                Body = new CreateTractorBeamEmitterTarget()
                {
                    TractorBeamEmitterKey = @event.Body.TractorBeamEmitterKey,
                    ShipPart = _shipParts.Get(targetId).Clone()
                }
            });

            @event.Simulation.Publish(new SimulationEventData()
            {
                Key = @event.NewKey(),
                SenderId = @event.SenderId,
                Body = new DestroyShipPartEntity()
                {
                    ShipPartEntityKey = parallelable.Key
                }
            });
        }

        public void Process(ISimulationEvent<CreateTractorBeamEmitterTarget> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            Entity target = @event.Simulation.CreateShipPart(@event.NewKey(), @event.Body.ShipPart);
            tractorBeamEmitter.TargetId = target.Id;
        }

        public void Process(ISimulationEvent<DeactivateTractorBeamEmitter> @event)
        {
            if (!@event.Simulation.TryGetEntityId(@event.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetId = null;
        }

        private bool Query(
            ISimulation simulation,
            TractorBeamEmitter emitter,
            Vector2 target,
            out int targetId)
        {
            const float Radius = 5f;
            AABB aabb = new AABB(target, Radius, Radius);
            float minDistance = Radius;
            int? callbackTargetId = default!;

            simulation.Aether.QueryAABB(fixture =>
            {
                if (fixture.Body.Tag is not int entityId)
                { // Invalid target - not an entity
                    return true;
                }

                if(!_tractorables.Has(entityId))
                { // Invalid target - not tractorable
                    return true;
                }

                if(!_worldLocations.TryGet(entityId, out WorldLocation? location))
                { // Invalid target - not in the world?
                    return true;
                }

                float distance = Vector2.Distance(target, location.Position);
                if (distance >= minDistance)
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                minDistance = distance;
                callbackTargetId = entityId;

                return true;
            }, ref aabb);

            if (callbackTargetId is null)
            {
                targetId = default;
                return false;
            }

            targetId = callbackTargetId.Value;
            return true;
        }
    }
}
