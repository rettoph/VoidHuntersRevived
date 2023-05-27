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
        private IParallelComponentMapper<TractorBeamEmitter> _tractorBeamEmitters = null!;
        private IParallelComponentMapper<Tactical> _tacticals = null!;
        private IParallelComponentMapper<Tractorable> _tractorables = null!;
        private IParallelComponentMapper<ShipPart> _shipParts = null!;
        private IParallelComponentMapper<IBody> _bodies = null!;

        public TractorBeamEmitterSystem(ILogger logger) : base(TractorBeamEmitterAspect)
        {
            _logger = logger;
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _tractorBeamEmitters = components.GetMapper<TractorBeamEmitter>();
            _tacticals = components.GetMapper<Tactical>();
            _tractorables = components.GetMapper<Tractorable>();
            _shipParts = components.GetMapper<ShipPart>();
            _bodies = components.GetMapper<IBody>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, ParallelKey entityKey)
        {
            if (!_tractorBeamEmitters.TryGet(entityKey, simulation, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetKey is null)
            {
                return;
            }

            if (!_tacticals.TryGet(entityKey, simulation, out Tactical? tactical))
            {
                return;
            }

            if(!_bodies.TryGet(tractorBeamEmitter.TargetKey.Value, simulation, out IBody? body))
            {
                // _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)} - Invalid TractorBeamEmitter Id '{tractorBeamEmitter.TargetId.Value}'");
                return;
            }

            body.SetTransform(tactical.Value, Fix64.Zero);
        }

        public void Process(in ISimulationEvent<ActivateTractorBeamEmitter> message)
        {
            if (!_tractorBeamEmitters.TryGet(message.Body.TractorBeamEmitterKey, message.Simulation, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetKey is not null)
            {
                return;
            }

            if (!_tacticals.TryGet(message.Body.TractorBeamEmitterKey, message.Simulation, out Tactical? tactical))
            {
                return;
            }

            if (!this.Query(message.Simulation, tractorBeamEmitter, tactical.Value, out ParallelKey targetKey))
            { // No valid potential targets found...
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(ActivateTractorBeamEmitter)}> - Activating {message.Body.TractorBeamEmitterKey}, TargetId: {targetKey}");

            // Create a clone of the target piece
            IBody body = _bodies.Get(targetKey, message.Simulation);
            ParallelKey cloneKey = message.Key.Step(1);
            int clone = message.Simulation.CreateShipPart(
                key: cloneKey, 
                shipPart: _shipParts.Get(targetKey, message.Simulation).Clone(), 
                tractorable: false,
                position: body.Position,
                rotation: body.Rotation);

            // Select the brand new clone
            message.Simulation.Publish(new SimulationEventData()
            {
                Key = message.Key.Step(2),
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

            if (!_tractorBeamEmitters.TryGet(message.Body.TractorBeamEmitterKey, message.Simulation, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetKey = message.Body.TargetKey;
        }

        public void Process(in ISimulationEventRevision<SetTractorBeamEmitterTarget> message)
        {
            if (!_tractorBeamEmitters.TryGet(message.Body.TractorBeamEmitterKey, message.Simulation, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            tractorBeamEmitter.TargetKey = null;
        }

        public void Process(in ISimulationEvent<DeactivateTractorBeamEmitter> message)
        {
            if (!_tractorBeamEmitters.TryGet(message.Body.TractorBeamEmitterKey, message.Simulation, out TractorBeamEmitter? tractorBeamEmitter))
            {
                return;
            }

            if(tractorBeamEmitter.TargetKey == null)
            {
                return;
            }

            if(!_bodies.TryGet(tractorBeamEmitter.TargetKey.Value, message.Simulation, out IBody? body))
            {
                _logger.Error($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Invalid TractorBeamEmitter Target Id, {tractorBeamEmitter.TargetKey.Value}");
                return;
            }

            _logger.Debug($"{nameof(TractorBeamEmitterSystem)}::{nameof(Process)}<{nameof(DeactivateTractorBeamEmitter)}> - Deactivating {tractorBeamEmitter.TargetKey.Value}, TargetId: {tractorBeamEmitter.TargetKey}");

            int clone = message.Simulation.CreateShipPart(
                key: message.Key.Step(1), 
                shipPart: _shipParts.Get(tractorBeamEmitter.TargetKey.Value, message.Simulation).Clone(), 
                tractorable: true,
                position: body.Position,
                rotation: body.Rotation);

            // Destroy the old target
            message.Simulation.DestroyEntity(tractorBeamEmitter.TargetKey.Value);

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
                if(!_tractorables.Has(fixture.Body.EntityKey, simulation))
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
