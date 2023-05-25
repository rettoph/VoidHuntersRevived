using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Physics;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using tainicom.Aether.Physics2D.Common;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : ParallelEntitySystem, ISimulationUpdateSystem, IPredictiveSynchronizationSystem
    {
        public static readonly AspectBuilder BodyAspect = Aspect.All(new[]
{
            typeof(ISimulation),
            typeof(IBody),
            typeof(Parallelable)
        });

        private ComponentMapper<ISimulation> _simulations = null!;
        private ComponentMapper<IBody> _bodies = null!;
        private ComponentMapper<Parallelable> _parallelables = null!;

        public SpaceSystem() : base(BodyAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _simulations = mapperService.GetMapper<ISimulation>();
            _bodies = mapperService.GetMapper<IBody>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        public void Update(ISimulation simulation, GameTime gameTime)
        {
            simulation.Space.Step(gameTime.ElapsedGameTime);
        }

        public void Synchronize(ISimulation simulation, GameTime gameTime, Fix64 damping)
        {
            foreach (int entityId in this.Entities[simulation.Type].ActiveEntities)
            {
                if (!_parallelables.Get(entityId).TryGetId(SimulationType.Lockstep, out int lockstepEntityId))
                {
                    continue;
                }

                IBody predictiveBody = _bodies.Get(entityId);
                IBody lockstepBody = _bodies.Get(lockstepEntityId);
                if (predictiveBody is null || lockstepBody is null)
                {
                    return;
                }

                predictiveBody.SetTransform(
                    position: FixVector2.Lerp(predictiveBody.Position, lockstepBody.Position, damping),
                    rotation: Fix64.Lerp(predictiveBody.Rotation, lockstepBody.Rotation, damping));

                predictiveBody.SetVelocity(
                    linear: FixVector2.Lerp(predictiveBody.LinearVelocity, lockstepBody.LinearVelocity, damping),
                    angular: Fix64.Lerp(predictiveBody.AngularVelocity, lockstepBody.AngularVelocity, damping));
            }
        }
    }
}
