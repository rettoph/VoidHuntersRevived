using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class FilterContextHelper
    {
        private static class FilterContext<TComponent, TFilter>
            where TComponent : unmanaged, IEntityComponent
        {
            public static FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public static FilterContextID GetFilterContext<TComponent, TFilter>()
            where TComponent : unmanaged, IEntityComponent
        {
            return FilterContext<TComponent, TFilter>.Value;
        }
    }
}
