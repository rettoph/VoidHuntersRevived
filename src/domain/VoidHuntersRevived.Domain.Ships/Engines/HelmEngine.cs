using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Ships.Engines
{
    public sealed class HelmEngine(
        IEntityQueryService entityQueryService) : StrategyEngine,
        IEventEngine<Helm_SetDirection>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void Process(VhId vhid, Helm_SetDirection data)
        {
            EntityLocalId shipLocalId = _entityQueryService.GetLocalId(data.ShipGlobalId);
            ref Helm helm = ref _entityQueryService.QueryByLocalId<Helm>(shipLocalId);

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
