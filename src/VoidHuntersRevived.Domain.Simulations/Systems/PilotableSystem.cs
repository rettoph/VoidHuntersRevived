using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using Serilog;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class PilotableSystem : ParallelEntityProcessingSystem, 
        ISimulationUpdateSystem,
        IPredictiveSynchronizationSystem
    {
        private ComponentMapper<Parallelable> _parallelables;
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<Body> _bodies;
        private ILogger _logger;

        public PilotableSystem(ILogger logger, ISimulationService simulations) : base(simulations, Aspect.All(typeof(Pilotable)))
        {
            _logger = logger;
            _pilotables = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _parallelables = mapperService.GetMapper<Parallelable>();
            _pilotables = mapperService.GetMapper<Pilotable>();
            _bodies = mapperService.GetMapper<Body>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);

            if (pilotable is null || pilotable.Direction == Direction.None)
            {
                return;
            }

            var body = _bodies.Get(entityId);
            if(body is null)
            {
                return;
            }

            var impulse = Vector2.Zero;
            var angularImpulse = 0f;

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
            body.ApplyAngularImpulse(angularImpulse);
        }

        public void Synchronize(ISimulation simulation, GameTime gameTime, float damping)
        {
            foreach (int entityId in this.Entities[simulation.Type].ActiveEntities)
            {
                var lockstepEntityId = _parallelables.Get(entityId).GetId(SimulationType.Lockstep);

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

                body.SetTransformIgnoreContacts(
                    position: Vector2.Lerp(body.Position, lockstepBody.Position, damping),
                    angle: MathHelper.Lerp(body.Rotation, lockstepBody.Rotation, damping));

                body.LinearVelocity = Vector2.Lerp(body.LinearVelocity, lockstepBody.LinearVelocity, damping);
                body.AngularVelocity = MathHelper.Lerp(body.AngularVelocity, lockstepBody.AngularVelocity, damping);
            }
        }
    }
}
