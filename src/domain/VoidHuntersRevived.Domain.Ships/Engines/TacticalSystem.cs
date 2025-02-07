using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Ships.Systems
{
    public sealed class TacticalSystem(
        IEntityQueryService entityQueryService
    ) : ISceneSystem,
        IEventSystem<Tactical_SetTarget>,
        IOnStepSystem
    {
        private static readonly Fix64 _aimDamping = Fix64.One / (Fix64)32;

        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void Process(VhId eventId, Tactical_SetTarget data)
        {
            EntityLocalId shipLocalId = this._entityQueryService.GetLocalId(data.ShipGlobalId);
            ref Tactical tactical = ref this._entityQueryService.QueryByLocalId<Tactical>(shipLocalId);

            tactical.Target = data.Value;

            if (data.Snap)
            {
                tactical.Value = data.Value;
            }
        }

        [SequenceGroup<OnStepSequenceGroupEnum>(OnStepSequenceGroupEnum.SyncronizeEntities)]
        public void OnStep(Step step)
        {
            foreach (var ((tacticals, count), _) in this._entityQueryService.QueryEntities<Tactical>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref Tactical tactical = ref tacticals[i];

                    Fix64 amount = Fix64.Min(step.ElapsedTime / _aimDamping, Fix64.One);
                    tactical.Value = FixVector2.Lerp(
                        v1: tactical.Value,
                        v2: tactical.Target,
                        amount: amount);
                }
            }
        }
    }
}