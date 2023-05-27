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
using VoidHuntersRevived.Common.Simulations.Services;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    internal sealed class SpaceSystem : ParallelEntitySystem, ISimulationUpdateSystem, IPredictiveSynchronizationSystem
    {
        public static readonly AspectBuilder BodyAspect = Aspect.All(new[]
{
            typeof(ISimulation),
            typeof(IBody)
        });

        private IParallelComponentMapper<ISimulation> _simulations = null!;
        private IParallelComponentMapper<IBody> _bodies = null!;

        public SpaceSystem() : base(BodyAspect)
        {
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _simulations = components.GetMapper<ISimulation>();
            _bodies = components.GetMapper<IBody>();
        }

        public void Update(ISimulation simulation, GameTime gameTime)
        {
            simulation.Space.Step(gameTime.ElapsedGameTime);
        }

        public void Synchronize(ISimulation predctive, ISimulation lockstep, GameTime gameTime, Fix64 damping)
        {
            foreach (ParallelKey entityKey in this.Entities[predctive.Type].ActiveEntities)
            {
                if (!_bodies.TryGet(entityKey, lockstep, out IBody? lockstepBody))
                {
                    return;
                }

                IBody predictiveBody = _bodies.Get(entityKey, predctive);

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
