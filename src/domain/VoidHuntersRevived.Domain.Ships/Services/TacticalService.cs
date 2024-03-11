using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Services;

namespace VoidHuntersRevived.Domain.Ships.Services
{
    internal class TacticalService : ITacticalService
    {
        private readonly IEntityService _entities;

        public TacticalService(IEntityService entities)
        {
            _entities = entities;
        }

        public void AddUse(EntityId tacticalId)
        {
            _entities.QueryById<Tactical>(tacticalId).AddUse();
        }

        public void RemoveUse(EntityId tacticalId)
        {
            _entities.QueryById<Tactical>(tacticalId).RemoveUse();
        }
    }
}
