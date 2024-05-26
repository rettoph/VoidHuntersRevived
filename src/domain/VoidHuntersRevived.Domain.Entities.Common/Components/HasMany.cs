using Guppy.Core.Common.Utilities;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Services;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public unsafe struct HasMany<TChild> : IEntityComponent
        where TChild : unmanaged, IEntityComponent
    {
        private static readonly FilterContextID _context = FilterContextID.GetNewContextID();

        private readonly UnmanagedReference<IEntityService> _entities;

        public CombinedFilterID FilterId;

        public HasMany(UnmanagedReference<IEntityService> entities)
        {
            _entities = entities;
        }
    }
}
