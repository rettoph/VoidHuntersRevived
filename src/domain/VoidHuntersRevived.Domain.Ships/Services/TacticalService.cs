using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    internal class TacticalService(IEntityQueryService entityQueryService) : ITacticalService
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        public void AddUse(EntityId tacticalId)
        {
            _entityQueryService.QueryById<Tactical>(tacticalId).AddUse();
        }

        public void RemoveUse(EntityId tacticalId)
        {
            _entityQueryService.QueryById<Tactical>(tacticalId).RemoveUse();
        }
    }
}
