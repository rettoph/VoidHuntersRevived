using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct HasMany<TItems, TAs> : IEntityComponent
        where TItems : unmanaged, IEntityComponent
        where TAs : unmanaged, IEntityComponent
    {
        private static readonly FilterContextID _context = FilterContextID.GetNewContextID();

        private readonly UnmanagedReference<IEntityService> _entities;
        private readonly CombinedFilterID _filterId;

        public EntityFilterCollection Items => _entities.Value.Query.GetFilter<TItems>(_filterId);

        public HasMany(EntityId id, UnmanagedReference<IEntityService> entities)
        {
            _filterId = new CombinedFilterID(unchecked((int)id.EGID.entityID), _context);
            _entities = entities;
        }
    }
}
