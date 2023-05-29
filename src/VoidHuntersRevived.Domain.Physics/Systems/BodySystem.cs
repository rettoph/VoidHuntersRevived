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
using Guppy.Common.Attributes;

namespace VoidHuntersRevived.Domain.Physics.Systems
{
    [Sortable<BodySystem>(int.MaxValue)]
    internal sealed class BodySystem : ParallelEntitySystem, IPredictiveSynchronizationSystem
    {
        public static readonly AspectBuilder BodyAspect = Aspect.All(new[]
        {
            typeof(ISimulation),
            typeof(IBody)
        });

        private IParallelComponentMapper<IBody> _bodies = null!;

        public BodySystem() : base(BodyAspect)
        {
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _bodies = components.GetMapper<IBody>();
        }

        protected override void OnEntityAdded(ParallelKey entityKey, ISimulation simulation)
        {
            base.OnEntityAdded(entityKey, simulation);

            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            IBody body = _bodies.Get(entityKey, simulation);

            if(body.Space != simulation.Space)
            {
                body.Space?.Remove(body);
                simulation.Space.Add(body);
            }
        }

        protected override void OnEntityRemoved(ParallelKey entityKey, ISimulation simulation)
        {
            base.OnEntityRemoved(entityKey, simulation);

            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            IBody body = _bodies.Get(entityKey, simulation);
            body.Space?.Remove(body);
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
