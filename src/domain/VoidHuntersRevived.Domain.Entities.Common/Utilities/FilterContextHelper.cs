using Svelto.ECS;

namespace VoidHuntersRevived.Domain.Entities.Common.Utilities
{
    public static class FilterContextHelper
    {
        private static class FilterContext<T1, T2>
        {
            public static FilterContextID Value = FilterContextID.GetNewContextID();
        }

        private static class FilterContext<T1, T2, T3>
        {
            public static FilterContextID Value = FilterContextID.GetNewContextID();
        }

        public static FilterContextID GetFilterContext<T1, T2>()
        {
            return FilterContext<T1, T2>.Value;
        }

        public static FilterContextID GetFilterContext<T1, T2, T3>()
        {
            return FilterContext<T1, T2, T3>.Value;
        }
    }
}
