using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Serilog;
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
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class TractorBeamEmitterSystem : ParallelEntityProcessingSystem,
        ISubscriber<ISimulationEvent<ActivateTractorBeamEmitter>>,
        ISubscriber<ISimulationEvent<SetTractorBeamEmitterTarget>>,
        ISubscriber<ISimulationEventRevision<SetTractorBeamEmitterTarget>>,
        ISubscriber<ISimulationEvent<DeactivateTractorBeamEmitter>>
    {
        private static readonly AspectBuilder TractorBeamEmitterAspect = Aspect.All(new[]
        {
            typeof(TractorBeamEmitter),
            typeof(Tactical)
        });

        private ILogger _logger;
        private ComponentMapper<TractorBeamEmitter> _tractorBeamEmitters = null!;
        private ComponentMapper<Tactical> _tacticals = null!;
        private ComponentMapper<Tractorable> _tractorables = null!;
        private ComponentMapper<ShipPart> _shipParts = null!;
        private ComponentMapper<WorldLocation> _worldLocations = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public TractorBeamEmitterSystem(ILogger logger, ISimulationService simulations) : base(simulations, TractorBeamEmitterAspect)
        {
            _logger = logger;
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

            if(!_worldLocations.TryGet(tractorBeamEmitter.TargetId.Value, out WorldLocation? location))
            {
                // _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)} - Invalid TractorBeamEmitter Id '{tractorBeamEmitter.TargetId.Value}'");
                return;
            }

            location.SetTransform(tactical.Value, 0);
        }

        public void Process(in ISimulationEvent<ActivateTractorBeamEmitter> message)
        {
            if(!message.Simulation.TryGetEntityId(message.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
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

            if (!this.Query(message.Simulation, tractorBeamEmitter, tactical.Value, out int targetId))
            { // No valid potential targets found...
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(ActivateTractorBeamEmitter)}> - Activating {tractorBeamEmitterId}, TargetId: {targetId}");

            // Create a clone of the target piece
            WorldLocation location = _worldLocations.Get(targetId);
            Entity clone = message.Simulation.CreateShipPart(
                key: message.NewKey(), 
                shipPart: _shipParts.Get(targetId).Clone(), 
                tractorable: false,
                position: location.Position,
                rotation: location.Rotation);

            // Select the brand new clone
            message.Simulation.Publish(new SimulationEventData()
            {
                Key = message.NewKey(),
                SenderId = message.SenderId,
                Body = new SetTractorBeamEmitterTarget()
                {
                    TractorBeamEmitterKey = message.Body.TractorBeamEmitterKey,
                    TargetKey = _parallelables.Get(clone).Key
                }
            });

            // Destroy the old piece
            ParallelKey targetKey = _parallelables.Get(targetId).Key;
            message.Simulation.DestroyEntity(targetKey);
        }

        public void Process(in ISimulationEvent<SetTractorBeamEmitterTarget> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if(!message.Simulation.TryGetEntityId(message.Body.TargetKey, out int targetId))
            {

            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetId = targetId;
        }

        public void Process(in ISimulationEventRevision<SetTractorBeamEmitterTarget> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetId = null;
        }

        public void Process(in ISimulationEvent<DeactivateTractorBeamEmitter> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Body.TractorBeamEmitterKey, out int tractorBeamEmitterId))
            {
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetId == null)
            {
                return;
            }

            if(!_worldLocations.TryGet(tractorBeamEmitter.TargetId.Value, out WorldLocation? location))
            {
                _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Invalid TractorBeamEmitter Target Id, {tractorBeamEmitter.TargetId.Value}");
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Deactivating {tractorBeamEmitterId}, TargetId: {tractorBeamEmitter.TargetId}");

            Entity clone = message.Simulation.CreateShipPart(
                key: message.NewKey(), 
                shipPart: _shipParts.Get(tractorBeamEmitter.TargetId.Value).Clone(), 
                tractorable: true,
                position: location.Position,
                rotation: location.Rotation);

            // Destroy the old target
            ParallelKey targetKey = _parallelables.Get(tractorBeamEmitter.TargetId.Value).Key;
            message.Simulation.DestroyEntity(targetKey);

            // Remove the reference
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
