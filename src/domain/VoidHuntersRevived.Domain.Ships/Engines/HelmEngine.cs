using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    [AutoLoad]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    internal sealed class HelmEngine : StrategyEngine,
        IEventEngine<Helm_SetDirection>
    {
        private readonly IEntityQueryService _entityQueryService;

        public HelmEngine(
            IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;
        }

        public void Process(VhId vhid, Helm_SetDirection data)
        {
            EntityId id = _entityQueryService.GetId(data.ShipVhId);
            ref Helm helm = ref _entityQueryService.QueryById<Helm>(id);

            if (data.Value)
            {
                helm.Direction |= data.Which;
            }
            else
            {
                helm.Direction &= ~data.Which;
            }
        }
    }
}
