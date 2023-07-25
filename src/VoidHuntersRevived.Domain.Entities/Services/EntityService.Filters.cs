using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal partial class EntityService
    {
        public ref EntityFilterCollection GetFilter<T>(EntityId id, FilterContextID filterContext)
            where T : unmanaged, IEntityComponent
        {
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(unchecked((int)id.EGID.entityID), filterContext);

            return ref filter;
        }
    }
}
