using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Entities.Events;

namespace VoidHuntersRevived.Domain.Entities.Systems
{
    internal sealed class TacticalSystem : ParallelEntityProcessingSystem,
        ISubscriber<ISimulationEvent<SetTacticalTarget>>
    {
        private const float AimDamping = 1f / 32f;
        private static readonly AspectBuilder TacticalAspect = Aspect.All(new[]
        {
            typeof(Tactical)
        });

        private ComponentMapper<Tactical> _tacticals = null!;

        public TacticalSystem() : base(TacticalAspect)
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _tacticals = mapperService.GetMapper<Tactical>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            Tactical tactical = _tacticals.Get(entityId);

            float amount = MathF.Min((float)gameTime.ElapsedGameTime.TotalSeconds / AimDamping, 1f);
            tactical.Value = Vector2.Lerp(
                value1: tactical.Value,
                value2: tactical.Target,
                amount: amount);
        }

        public void Process(in ISimulationEvent<SetTacticalTarget> message)
        {
            if (!message.Simulation.TryGetEntityId(message.Body.TacticalKey, out int tacticalId))
            {
                return;
            }

            if (!_tacticals.TryGet(tacticalId, out Tactical? tactical))
            {
                return;
            }

            tactical.Target = message.Body.Target;
        }
    }
}
