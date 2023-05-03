using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common.Simulations
{
    public sealed class ParallelKeyGenerator
    {
        private readonly int DataSizeInBytes = sizeof(ulong) * 3;

        private ParallelKey _key;

        public ParallelKey Current => _key;

        public unsafe ParallelKeyGenerator(ParallelKey source)
        {
            _key = source;
        }

        public ParallelKey Next()
        {
            return ++_key;
        }

        public ParallelKey Previous()
        {
            return --_key;
        }
    }
}
