using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    internal sealed class AetherSystem : ParallelEntitySystem, ISimulationUpdateSystem, IPredictiveSynchronizationSystem
    {
        public static readonly AspectBuilder BodyAspect = Aspect.All(new[]
        {
            typeof(ISimulation),
            typeof(Body)
        });

        private ComponentMapper<ISimulation> _simulations = null!;
        private ComponentMapper<Body> _bodies = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public AetherSystem(ISimulationService simulations) : base(simulations, BodyAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _simulations = mapperService.GetMapper<ISimulation>();
            _bodies = mapperService.GetMapper<Body>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if (!this.subscription.IsInterested(entityId))
            {
                return;
            }

            Aether world = _simulations.Get(entityId).Aether;
            Body body = _bodies.Get(entityId);

            if(body.World != world)
            {
                body.World?.Remove(body);
                world.Add(body);
            }

            body.Tag = entityId;
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            Body body = _bodies.Get(entityId);
            body.World.Remove(body);
            body.Tag = null;
        }

        public void Update(ISimulation simulation, GameTime gameTime)
        {
            simulation.Aether.Step(gameTime.ElapsedGameTime);
        }

        public void Synchronize(ISimulation simulation, GameTime gameTime, float damping)
        {
            foreach (int entityId in this.Entities[simulation.Type].ActiveEntities)
            {
                if(!_parallelables.Get(entityId).TryGetId(SimulationType.Lockstep, out int lockstepEntityId))
                {
                    continue;
                }

                Body predictiveBody = _bodies.Get(entityId);
                Body lockstepBody = _bodies.Get(lockstepEntityId);
                if (predictiveBody is null || lockstepBody is null)
                {
                    return;
                }

                predictiveBody.SetTransformIgnoreContacts(
                    position: Vector2.Lerp(predictiveBody.Position, lockstepBody.Position, damping),
                    angle: MathHelper.Lerp(predictiveBody.Rotation, lockstepBody.Rotation, damping));

                predictiveBody.LinearVelocity = Vector2.Lerp(predictiveBody.LinearVelocity, lockstepBody.LinearVelocity, damping);
                predictiveBody.AngularVelocity = MathHelper.Lerp(predictiveBody.AngularVelocity, lockstepBody.AngularVelocity, damping);
            }
        }
    }
}
