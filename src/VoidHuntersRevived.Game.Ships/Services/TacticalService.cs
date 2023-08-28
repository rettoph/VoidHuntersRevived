using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;

namespace VoidHuntersRevived.Game.Ships.Services
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
