using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common.Simulations
{
    public sealed class ParallelKeyFactory
    {
        private readonly int DataSizeInBytes = sizeof(ulong) * 3;

        private ParallelKey _key;

        public ParallelKey Current => _key;

        public unsafe ParallelKeyFactory(ParallelKey source)
        {
            _key = source;
        }

        public ParallelKey Next()
        {
            _key = _key.Next();
            return _key;
        }

        public ParallelKey Previous()
        {
            _key = _key.Previous();
            return _key;
        }
    }
}
