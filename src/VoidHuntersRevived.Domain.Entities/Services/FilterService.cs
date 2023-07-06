using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Services
{
    internal sealed class FilterService : IFilterService, IQueryingEntitiesEngine
    {
        private int _nextFilterId;
        private readonly FilterContextID _filterContextId = new FilterContextID();
        private readonly Dictionary<VhId, CombinedFilterID> _filterIds = new Dictionary<VhId, CombinedFilterID>();

        public EntitiesDB entitiesDB { get; set; } = null!;

        public void Ready()
        {

        }

        public ref EntityFilterCollection GetFilter<T>(VhId filterVhId)
            where T : unmanaged, IEntityComponent
        {
            ref CombinedFilterID filterId = ref this.GetOrCreateFilterId(filterVhId);
            ref var filter = ref this.entitiesDB.GetFilters().GetOrCreatePersistentFilter<T>(filterId);

            return ref filter;
        }

        private ref CombinedFilterID GetOrCreateFilterId(VhId filterVhId)
        {
            ref CombinedFilterID combinedFilterId = ref CollectionsMarshal.GetValueRefOrAddDefault(_filterIds, filterVhId, out bool exists);

            if(!exists)
            {
                combinedFilterId = new CombinedFilterID(_nextFilterId++, _filterContextId);
            }

            return ref combinedFilterId;
        }
    }
}
