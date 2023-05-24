using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class HelmBodySystem : ParallelEntityProcessingSystem
    {
        public static readonly AspectBuilder HelmBodyAspect = Aspect.All(new[]
        {
            typeof(Helm),
            typeof(Body)
        });

        private ComponentMapper<Helm> _helms = null!;
        private ComponentMapper<Body> _body = null!;

        public HelmBodySystem() : base(HelmBodyAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _helms = mapperService.GetMapper<Helm>();
            _body = mapperService.GetMapper<Body>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            Helm helm = _helms.Get(entityId);
            Body body = _body.Get(entityId);

            Vector2 impulse = Vector2.Zero;
            float angularImpulse = 0f;

            if (helm.Direction.HasFlag(Direction.Forward))
            {
                impulse -= Vector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.Backward))
            {
                impulse += Vector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.TurnLeft))
            {
                impulse -= Vector2.UnitX;
            }

            if (helm.Direction.HasFlag(Direction.TurnRight))
            {
                impulse += Vector2.UnitX;
            }

            impulse *= (float)gameTime.ElapsedGameTime.TotalSeconds;

            body.ApplyLinearImpulse(impulse);
            body.ApplyAngularImpulse(angularImpulse);
        }
    }
}
