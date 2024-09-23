using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    [Sequence<StepSequence>(StepSequence.PreStep)]
    internal sealed class TacticalEngine : StrategyEngine,
        IEventEngine<Tactical_SetTarget>,
        IStepEngine<Step>
    {
        private static readonly Fix64 AimDamping = Fix64.One / (Fix64)32;

        private readonly IEntityQueryService _entityQueryService;

        public TacticalEngine(
            IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public string name { get; } = nameof(TacticalEngine);

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

        public void Step(in Step _param)
        {
            foreach (var ((tacticals, count), groupId) in _entityQueryService.QueryEntities<Tactical>())
            {
                for (int i = 0; i < count; i++)
                {
                    ref Tactical tactical = ref tacticals[i];

                    Fix64 amount = Fix64.Min(_param.ElapsedTime / AimDamping, Fix64.One);
                    tactical.Value = FixVector2.Lerp(
                        v1: tactical.Value,
                        v2: tactical.Target,
                        amount: amount);
                }
            }
        }
    }
}
