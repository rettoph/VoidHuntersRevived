using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Common.Utilities
{
    public static class HashBuilderExtensions
    {
        public static Id<TNamespace> CalculateId<TNamespace, TName1>(
            this HashBuilder<TNamespace, TName1> builder,
            in TName1 value1
        )
            where TName1 : unmanaged
        {
            return new Id<TNamespace>(builder.Calculate(value1));
        }

        public static Id<TNamespace> CalculateId<TNamespace, TName1, TName2>(
            this HashBuilder<TNamespace, TName1, TName2> builder,
            in TName1 value1,
            in TName2 value2
        )
            where TName1 : unmanaged
            where TName2 : unmanaged
        {
            return new Id<TNamespace>(builder.Calculate(value1, value2));
        }

        public static Id<TNamespace> CalculateId<TNamespace, TName1, TName2, TName3>(
            this HashBuilder<TNamespace, TName1, TName2, TName3> builder,
            in TName1 value1,
            in TName2 value2,
            in TName3 value3
        )
            where TName1 : unmanaged
            where TName2 : unmanaged
            where TName3 : unmanaged
        {
            return new Id<TNamespace>(builder.Calculate(value1, value2, value3));
        }

        public static Id<TNamespace> CalculateId<TNamespace, TName1, TName2, TName3, TName4>(
            this HashBuilder<TNamespace, TName1, TName2, TName3, TName4> builder,
            in TName1 value1,
            in TName2 value2,
            in TName3 value3,
            in TName4 value4
        )
            where TName1 : unmanaged
            where TName2 : unmanaged
            where TName3 : unmanaged
            where TName4 : unmanaged
        {
            return new Id<TNamespace>(builder.Calculate(in value1, in value2, in value3, in value4));
        }

        public static Id<TNamespace> CalculateId<TNamespace, TName1, TName2, TName3, TName4, TName5>(
            this HashBuilder<TNamespace, TName1, TName2, TName3, TName4, TName5> builder,
            in TName1 value1,
            in TName2 value2,
            in TName3 value3,
            in TName4 value4,
            in TName5 value5
        )
            where TName1 : unmanaged
            where TName2 : unmanaged
            where TName3 : unmanaged
            where TName4 : unmanaged
            where TName5 : unmanaged
        {
            return new Id<TNamespace>(builder.Calculate(in value1, in value2, in value3, in value4, in value5));
        }
    }
}
