using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    internal sealed class TacticalEngine(
        IEntityQueryService entityQueryService) : StrategyEngine,
        IEventEngine<Tactical_SetTarget>,
        IOnStepEngine
    {
        private static readonly Fix64 AimDamping = Fix64.One / (Fix64)32;

        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void Process(VhId eventId, Tactical_SetTarget data)
        {
            EntityId id = _entityQueryService.GetId(data.ShipVhId);
            ref Tactical tactical = ref _entityQueryService.QueryById<Tactical>(id);

            tactical.Target = data.Value;

            if (data.Snap)
            {
                tactical.Value = data.Value;
            }
        }

        [SequenceGroup<OnStepSequenceGroup>(OnStepSequenceGroup.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((tacticals, count), groupId) in _entityQueryService.QueryEntities<Tactical>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref Tactical tactical = ref tacticals[i];

                    Fix64 amount = Fix64.Min(step.ElapsedTime / AimDamping, Fix64.One);
                    tactical.Value = FixVector2.Lerp(
                        v1: tactical.Value,
                        v2: tactical.Target,
                        amount: amount);
                }
            }
        }
    }
}
