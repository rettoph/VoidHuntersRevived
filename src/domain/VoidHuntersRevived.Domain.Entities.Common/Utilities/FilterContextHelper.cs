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

        private static class FilterContext<TComponent1, TComponent2, TFilter>
            where TComponent1 : unmanaged, IEntityComponent
            where TComponent2 : unmanaged, IEntityComponent
        {
            public static FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public static FilterContextID GetFilterContext<TComponent, TFilter>()
            where TComponent : unmanaged, IEntityComponent
        {
            return FilterContext<TComponent, TFilter>.Value;
        }

        public static FilterContextID GetFilterContext<TComponent1, TComponent2, TFilter>()
            where TComponent1 : unmanaged, IEntityComponent
            where TComponent2 : unmanaged, IEntityComponent
        {
            return FilterContext<TComponent1, TComponent2, TFilter>.Value;
        }
    }
}
