using Guppy.Core.Common.Utilities;
using Svelto.ECS;

namespace VoidHuntersRevived.Common.Entities.Components
{
    public struct HasMany<TChild> : IEntityComponent
        where TChild : unmanaged, IEntityComponent
    {
        private static readonly FilterContextID _context = FilterContextID.GetNewContextID();

        public CombinedFilterID FilterId;
    }
}
