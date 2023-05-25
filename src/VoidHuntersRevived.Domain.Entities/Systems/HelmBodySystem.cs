using FixedMath.NET;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Common;
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

            AetherVector2 impulse = AetherVector2.Zero;
            Fix64 angularImpulse = Fix64.Zero;

            if (helm.Direction.HasFlag(Direction.Forward))
            {
                impulse -= AetherVector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.Backward))
            {
                impulse += AetherVector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.TurnLeft))
            {
                impulse -= AetherVector2.UnitX;
            }

            if (helm.Direction.HasFlag(Direction.TurnRight))
            {
                impulse += AetherVector2.UnitX;
            }

            // TODO: Fix this - determinsim is lost by casting a float
            impulse *= (Fix64)gameTime.ElapsedGameTime.TotalSeconds;

            body.ApplyLinearImpulse(impulse);
            body.ApplyAngularImpulse(angularImpulse);
        }
    }
}
