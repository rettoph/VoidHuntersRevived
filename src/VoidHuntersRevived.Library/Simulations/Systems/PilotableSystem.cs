using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Services;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Library.Simulations.Systems
{
    internal sealed class PilotableSystem : ParallelEntityProcessingSystem, 
        IUpdateSimulationSystem,
        ISynchronizationSystem
    {
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Body> _bodies;

        public PilotableSystem(ISimulationService simulations) : base(simulations, Aspect.All(typeof(Pilotable)))
        {
            _pilotables = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotables = mapperService.GetMapper<Pilotable>();
            _bodies = mapperService.GetMapper<Body>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            if(pilotable is null || pilotable.Direction == Direction.None)
            {
                return;
            }

            var body = _bodies.Get(entityId);
            if(body is null)
            {
                return;
            }

            var impulse = Vector2.Zero;

            if (pilotable.Direction.HasFlag(Direction.Forward))
            {
                impulse -= Vector2.UnitY;
            }

            if (pilotable.Direction.HasFlag(Direction.Backward))
            {
                impulse += Vector2.UnitY;
            }

            if (pilotable.Direction.HasFlag(Direction.TurnLeft))
            {
                impulse -= Vector2.UnitX;
            }

            if (pilotable.Direction.HasFlag(Direction.TurnRight))
            {
                impulse += Vector2.UnitX;
            }

            impulse *= (float)gameTime.ElapsedGameTime.TotalSeconds;

            body.ApplyLinearImpulse(impulse);
        }

        public void Synchronize(ISimulation simulation, GameTime gameTime)
        {
            foreach(int entityId in this.Entities[simulation.Type].ActiveEntities)
            {
                var lockstepEntityId = simulation.GetEntityId(entityId, SimulationType.Lockstep);

                var pilotable = _pilotables.Get(entityId);
                var lockstepPilotable = _pilotables.Get(lockstepEntityId);
                
                if (pilotable is null || lockstepPilotable is null)
                {
                    return;
                }

                var body = _bodies.Get(entityId);
                var lockstepBody = _bodies.Get(lockstepEntityId);
                if (body is null || lockstepBody is null)
                {
                    return;
                }

                var lerpStrength = 1f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                body.SetTransformIgnoreContacts(
                    position: Vector2.Lerp(body.Position, lockstepBody.Position, lerpStrength),
                    angle: MathHelper.Lerp(body.Rotation, lockstepBody.Rotation, lerpStrength));

                body.LinearVelocity = Vector2.Lerp(body.LinearVelocity, lockstepBody.LinearVelocity, lerpStrength);
                body.AngularVelocity = MathHelper.Lerp(body.AngularVelocity, lockstepBody.AngularVelocity, lerpStrength);
            }
        }
    }
}
