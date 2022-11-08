using Guppy.Attributes;
using Guppy.Resources.Providers;
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

namespace VoidHuntersRevived.Library.Systems
{
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class PilotableAetherSystem : EntityTickSystem
    {
        private ComponentMapper<Pilotable> _pilotables;
        private ComponentMapper<AetherBody> _bodies;

        public PilotableAetherSystem(ISettingProvider settings) : base(settings, Aspect.All(typeof(AetherBody), typeof(Pilotable)))
        {
            _pilotables = default!;
            _bodies = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotables = mapperService.GetMapper<Pilotable>();
            _bodies = mapperService.GetMapper<AetherBody>();
        }

        public override void Process(float elapsedTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);
            
            if(pilotable.Direction == Direction.None)
            {
                return;
            }

            var body = _bodies.Get(entityId);
            var impulse = Vector2.Zero;

            if(pilotable.Direction.HasFlag(Direction.Forward))
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

            impulse *= elapsedTime;

            body.ApplyLinearImpulse(impulse);
        }
    }
}
