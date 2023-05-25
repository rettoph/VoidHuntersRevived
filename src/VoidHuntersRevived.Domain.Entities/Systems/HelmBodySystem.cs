using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Physics;
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
            typeof(IBody)
        });

        private ComponentMapper<Helm> _helms = null!;
        private ComponentMapper<IBody> _body = null!;

        public HelmBodySystem() : base(HelmBodyAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _helms = mapperService.GetMapper<Helm>();
            _body = mapperService.GetMapper<IBody>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            Helm helm = _helms.Get(entityId);
            IBody body = _body.Get(entityId);

            FixVector2 impulse = FixVector2.Zero;
            Fix64 angularImpulse = Fix64.Zero;

            if (helm.Direction.HasFlag(Direction.Forward))
            {
                impulse -= FixVector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.Backward))
            {
                impulse += FixVector2.UnitY;
            }

            if (helm.Direction.HasFlag(Direction.TurnLeft))
            {
                impulse -= FixVector2.UnitX;
            }

            if (helm.Direction.HasFlag(Direction.TurnRight))
            {
                impulse += FixVector2.UnitX;
            }

            // TODO: Fix this - determinsim is lost by casting a float
            impulse *= (Fix64)gameTime.ElapsedGameTime.TotalSeconds;

            body.ApplyLinearImpulse(impulse);
            body.ApplyAngularImpulse(angularImpulse);
        }
    }
}
