using Standart.Hash.xxHash;

namespace VoidHuntersRevived.Common.Simulations
{
    public sealed class ParallelKeyProvider
    {
        private readonly int DataSizeInBytes = sizeof(ulong) * 3;

        private ulong[] _data;

        public unsafe ParallelKeyProvider(ParallelKey source)
        {
            ulong* pSource = (ulong*)&source;
            _data = new[]
            {
                ulong.MinValue,
                pSource[0],
                pSource[1],
            };
        }

        public ParallelKey Next()
        {
            _data[0]++;
            return this.Generate();
        }

        public ParallelKey Previous()
        {
            _data[0]--;
            return this.Generate();
        }

        private unsafe ParallelKey Generate()
        {
            fixed (ulong* pData = _data)
            {
                Span<byte> dataSpan = new Span<byte>((byte*)pData, DataSizeInBytes);
                uint128 hash = xxHash128.ComputeHash(dataSpan, DataSizeInBytes);
                Guid* value = (Guid*)&hash;

                return new ParallelKey(value[0]);
            }
        }
    }
}
