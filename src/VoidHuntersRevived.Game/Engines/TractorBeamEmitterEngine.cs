using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;

namespace VoidHuntersRevived.Game.Engines
{
    internal sealed class TractorBeamEmitterEngine : BasicEngine,
        IEventEngine<SetTractorBeamEmitterActive>
    {
        public void Process(VhId eventId, SetTractorBeamEmitterActive data)
        {
            IdMap id = this.Simulation.Entities.GetIdMap(data.ShipId);
            ref TractorBeamEmitter tractorBeamEmitter = ref entitiesDB.QueryMappedEntities<TractorBeamEmitter>(id.EGID.groupID).Entity(id.EGID.entityID);

            if(tractorBeamEmitter.Active == data.Value)
            {
                return;
            }

            if(data.Value)
            {
                this.ActivateTractorBeamEmitter(ref eventId, ref id, ref tractorBeamEmitter);
            }
        }

        private void ActivateTractorBeamEmitter(ref VhId eventId, ref IdMap shipId, ref TractorBeamEmitter tractorBeamEmitter)
        {
            throw new NotImplementedException();
        }
    }
}
