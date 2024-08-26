using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct HasMany<TItems, TAs> : IEntityComponent
        where TItems : unmanaged, IEntityComponent
        where TAs : unmanaged, IEntityComponent
    {
        private static int _filterId;
        private static readonly FilterContextID _context = FilterContextID.GetNewContextID();

        private readonly CombinedFilterID _combinedFilterId;

        public HasMany()
        {
            _combinedFilterId = new CombinedFilterID(_filterId++, _context);
        }

        public ref EntityFilterCollection GetItems(IEntityQueryService entityQueryService)
        {
            return ref entityQueryService.GetFilter<TItems>(_combinedFilterId);
        }
    }
}
