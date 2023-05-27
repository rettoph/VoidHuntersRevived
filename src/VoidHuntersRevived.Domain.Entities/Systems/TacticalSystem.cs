using Guppy.Common;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using VoidHuntersRevived.Common;
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
        private static readonly Fix64 AimDamping = Fix64.One / (Fix64)32;
        private static readonly AspectBuilder TacticalAspect = Aspect.All(new[]
        {
            typeof(Tactical)
        });

        private IParallelComponentMapper<Tactical> _tacticals = null!;

        public TacticalSystem() : base(TacticalAspect)
        {
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _tacticals = components.GetMapper<Tactical>();
        }

        protected override void Process(ISimulation simulation, GameTime gameTime, ParallelKey parallelKey)
        {
            Tactical tactical = _tacticals.Get(parallelKey, simulation);

            Fix64 amount = Fix64.Min((Fix64)gameTime.ElapsedGameTime.TotalSeconds / AimDamping, Fix64.One);
            tactical.Value = FixVector2.Lerp(
                v1: tactical.Value,
                v2: tactical.Target,
                amount: amount);
        }

        public void Process(in ISimulationEvent<SetTacticalTarget> message)
        {
            if (!_tacticals.TryGet(message.Body.TacticalKey, message.Simulation, out Tactical? tactical))
            {
                return;
            }

            tactical.Target = message.Body.Target;
        }
    }
}
