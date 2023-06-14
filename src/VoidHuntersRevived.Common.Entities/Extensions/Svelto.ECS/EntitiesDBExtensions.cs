using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Svelto.ECS
{
    public static class EntitiesDBExtensions
    {
        public static EntityReference GetEntityReference(this EntitiesDB entitiesDB, uint entityId, ExclusiveGroupStruct groupId)
        {
            return entitiesDB.GetEntityReference(new EGID(entityId, groupId));
        }
    }
}
