using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Entities.ShipParts.Components;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Common.Physics;
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
        private ComponentMapper<Parallelable> _parallelables = null!;
        private ComponentMapper<IBody> _bodies = null!;

        public TractorBeamEmitterSystem(ILogger logger) : base(TractorBeamEmitterAspect)
        {
            _logger = logger;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tractorBeamEmitters = mapperService.GetMapper<TractorBeamEmitter>();
            _tacticals = mapperService.GetMapper<Tactical>();
            _tractorables = mapperService.GetMapper<Tractorable>();
            _shipParts = mapperService.GetMapper<ShipPart>();
            _parallelables = mapperService.GetMapper<Parallelable>();
            _bodies = mapperService.GetMapper<IBody>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            if (!_tractorBeamEmitters.TryGet(entityId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetKey is null)
            {
                return;
            }

            if (!_tacticals.TryGet(entityId, out Tactical? tactical))
            {
                return;
            }

            if(!simulation.TryGetEntityId(tractorBeamEmitter.TargetKey.Value, out int targetId))
            {
                return;
            }

            if(!_bodies.TryGet(targetId, out IBody? body))
            {
                // _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)} - Invalid TractorBeamEmitter Id '{tractorBeamEmitter.TargetId.Value}'");
                return;
            }

            body.SetTransform(tactical.Value, Fix64.Zero);
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

            if(tractorBeamEmitter.TargetKey is not null)
            {
                return;
            }

            if (!_tacticals.TryGet(tractorBeamEmitterId, out Tactical? tactical))
            {
                return;
            }

            if (!this.Query(message.Simulation, tractorBeamEmitter, tactical.Value, out ParallelKey targetKey))
            { // No valid potential targets found...
                return;
            }

            if(!message.Simulation.TryGetEntityId(targetKey, out int targetId))
            {
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(ActivateTractorBeamEmitter)}> - Activating {tractorBeamEmitterId}, TargetId: {targetId}");

            // Create a clone of the target piece
            IBody body = _bodies.Get(targetId);
            ParallelKey cloneKey = message.NewKey();
            int clone = message.Simulation.CreateShipPart(
                key: cloneKey, 
                shipPart: _shipParts.Get(targetId).Clone(), 
                tractorable: false,
                position: body.Position,
                rotation: body.Rotation);

            // Select the brand new clone
            message.Simulation.Publish(new SimulationEventData()
            {
                Key = message.NewKey(),
                SenderId = message.SenderId,
                Body = new SetTractorBeamEmitterTarget()
                {
                    TractorBeamEmitterKey = message.Body.TractorBeamEmitterKey,
                    TargetKey = cloneKey
                }
            });

            // Destroy the old piece
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
                return;
            }

            if (!_tractorBeamEmitters.TryGet(tractorBeamEmitterId, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetKey = message.Body.TargetKey;
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

            tractorBeamEmitter.TargetKey = null;
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

            if(tractorBeamEmitter.TargetKey == null)
            {
                return;
            }

            if(!message.Simulation.TryGetEntityId(tractorBeamEmitter.TargetKey.Value, out int targetId))
            {
                return;
            }

            if(!_bodies.TryGet(targetId, out IBody? body))
            {
                _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Invalid TractorBeamEmitter Target Id, {tractorBeamEmitter.TargetKey.Value}");
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Deactivating {tractorBeamEmitterId}, TargetId: {tractorBeamEmitter.TargetKey}");

            int clone = message.Simulation.CreateShipPart(
                key: message.NewKey(), 
                shipPart: _shipParts.Get(targetId).Clone(), 
                tractorable: true,
                position: body.Position,
                rotation: body.Rotation);

            // Destroy the old target
            ParallelKey targetKey = _parallelables.Get(targetId).Key;
            message.Simulation.DestroyEntity(targetKey);

            // Remove the reference
            tractorBeamEmitter.TargetKey = null;
        }

        static readonly Fix64 Radius = (Fix64)5;
        private bool Query(
            ISimulation simulation,
            TractorBeamEmitter emitter,
            FixVector2 target,
            out ParallelKey targetKey)
        {
            
            AABB aabb = (AABB)new AABB(target, Radius, Radius);
            Fix64 minDistance = Radius;
            ParallelKey? callbackTargetKey = default!;

            simulation.Space.QueryAABB(fixture =>
            {
                if(!simulation.TryGetEntityId(fixture.Body.EntityKey, out int entityId))
                { // Invalid target - not an entity
                    return true;
                }

                if(!_tractorables.Has(entityId))
                { // Invalid target - not tractorable
                    return true;
                }

                FixVector2 position = fixture.Body.Position;
                FixVector2.Distance(ref target, ref position, out Fix64 distance);
                if (distance >= minDistance)
                { // Invalid Target - The distance is further away than the previously closest valid target
                    return true;
                }

                minDistance = distance;
                callbackTargetKey = fixture.Body.EntityKey;

                return true;
            }, ref aabb);

            if (callbackTargetKey is null)
            {
                targetKey = default;
                return false;
            }

            targetKey = callbackTargetKey.Value;
            return true;
        }
    }
}
