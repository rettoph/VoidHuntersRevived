using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Entites.Systems
{
    internal sealed class PilotableSystem : ParallelEntityProcessingSystem
    {
        private const float AimDamping = 1f / 32f;

        public static readonly AspectBuilder PilotableAspect = Aspect.All(new[]
        {
            typeof(Pilotable)
        });

        private ComponentMapper<Pilotable> _pilotables;

        public PilotableSystem(ISimulationService simulations) : base(simulations, PilotableAspect)
        {
            _pilotables = default!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _pilotables = mapperService.GetMapper<Pilotable>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, int entityId)
        {
            var pilotable = _pilotables.Get(entityId);

            pilotable.Aim.Value = Vector2.Lerp(
                value1: pilotable.Aim.Value, 
                value2: pilotable.Aim.Target,
                amount: (float)gameTime.ElapsedGameTime.TotalSeconds / AimDamping);
        }
    }
}
