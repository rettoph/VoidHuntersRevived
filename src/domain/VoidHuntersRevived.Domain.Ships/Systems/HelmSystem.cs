using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Domain.Ships.Systems
{
    public sealed class HelmSystem(
        IEntityQueryService entityQueryService
    ) : ISceneSystem,
        IEventSystem<Helm_SetDirection>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId vhid, Helm_SetDirection data)
        {
            EntityLocalId shipLocalId = this._entityQueryService.GetLocalId(data.ShipGlobalId);
            ref Helm helm = ref this._entityQueryService.QueryByLocalId<Helm>(shipLocalId);

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