using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    public class TacticalService(IEntityQueryService entityQueryService) : ITacticalService
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void AddUse(EntityLocalId tacticalId)
        {
            this._entityQueryService.QueryByLocalId<Tactical>(tacticalId).AddUse();
        }

        public void RemoveUse(EntityLocalId tacticalId)
        {
            this._entityQueryService.QueryByLocalId<Tactical>(tacticalId).RemoveUse();
        }
    }
}