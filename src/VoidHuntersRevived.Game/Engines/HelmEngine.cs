using Guppy.Attributes;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Game.Components;
using VoidHuntersRevived.Game.Events;
using VoidHuntersRevived.Game.Pieces.Properties;

namespace VoidHuntersRevived.Game.Engines
{
    [AutoLoad]
    internal sealed class HelmEngine : BasicEngine,
        IEventEngine<SetHelmDirection>
    {
        public string name { get; } = nameof(HelmEngine);

        public void Process(VhId vhid, SetHelmDirection data)
        {
            IdMap id = this.Simulation.Entities.GetIdMap(data.ShipId);
            ref Helm helm = ref entitiesDB.QueryMappedEntities<Helm>(id.EGID.groupID).Entity(id.EGID.entityID);

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
