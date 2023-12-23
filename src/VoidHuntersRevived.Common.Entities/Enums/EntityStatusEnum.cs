using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Enums
{
    public enum EntityStatusEnum
    {
        NotSpawned = 0,
        Spawned = 1,
        SoftDespawnEnqueued = 2,
        SoftDespawned = 3,
        HardDespawned = 4
    }
}
