using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Components
{
    public struct Parent<TChild> : IEntityComponent
        where TChild : unmanaged, IEntityComponent
    {
        private static readonly FilterContextID _context = FilterContextID.GetNewContextID();

        public CombinedFilterID FilterId;
    }
}
