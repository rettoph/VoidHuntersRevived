using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevivied.Library.Extensions.tainicom.Aether.Dynamics;

namespace VoidHuntersRevived.Library.Simulations.Systems.Predictive
{
    internal sealed class PredictivePilotableAetherSystem : EntityProcessingSystem, IPredictiveSimulationSystem
    {
        private ISimulationService _simulations;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<AetherBody> _bodies;

        public PredictivePilotableAetherSystem(ISimulationService simulations) : base(Aspect.All(typeof(AetherBody), typeof(Pilotable)).Exclude(typeof(Lockstepped)))
        {
            _simulations = simulations;
            _pilotables = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotables = mapperService.GetMapper<Pilotable>();
            _bodies = mapperService.GetMapper<AetherBody>();
        }

        public override void Process(GameTime gameTime, int entityId)
        {
            this.SyncLockstep(gameTime, entityId);
            this.ApplyImpulse(gameTime, entityId);
        }

        private void ApplyImpulse(GameTime gameTime, int entityId)
        {
            var predictivePilotable = _pilotables.Get(entityId);

            if (predictivePilotable.Direction == Direction.None)
            {
                return;
            }

            var predictiveBody = _bodies.Get(entityId);
            var impulse = Vector2.Zero;

            if (predictivePilotable.Direction.HasFlag(Direction.Forward))
            {
                impulse -= Vector2.UnitY;
            }

            if (predictivePilotable.Direction.HasFlag(Direction.Backward))
            {
                impulse += Vector2.UnitY;
            }

            if (predictivePilotable.Direction.HasFlag(Direction.TurnLeft))
            {
                impulse -= Vector2.UnitX;
            }

            if (predictivePilotable.Direction.HasFlag(Direction.TurnRight))
            {
                impulse += Vector2.UnitX;
            }

            impulse *= (float)gameTime.ElapsedGameTime.TotalSeconds;

            predictiveBody.ApplyLinearImpulse(impulse);
        }

        private void SyncLockstep(GameTime gameTime, int entityId)
        {
            if (!_simulations[SimulationType.Predictive].TryGetEntityId(entityId, SimulationType.Lockstep, out var lockstepEntityId))
            {
                return;
            }

            var predictiveBody = _bodies.Get(entityId);
            var lockstepBody = _bodies.Get(lockstepEntityId);

            if(predictiveBody is null || lockstepBody is null)
            {
                return;
            }

            var lerpStrength = 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            predictiveBody.SetTransformIgnoreContacts(
                position: Vector2.Lerp(predictiveBody.Position, lockstepBody.Position, lerpStrength),
                angle: MathHelper.Lerp(predictiveBody.Rotation, lockstepBody.Rotation, lerpStrength));

            predictiveBody.LinearVelocity = Vector2.Lerp(predictiveBody.LinearVelocity, lockstepBody.LinearVelocity, lerpStrength);
            predictiveBody.AngularVelocity = MathHelper.Lerp(predictiveBody.AngularVelocity, lockstepBody.AngularVelocity, lerpStrength);
        }
    }
}
