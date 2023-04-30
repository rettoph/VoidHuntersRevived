using Standart.Hash.xxHash;
using VoidHuntersRevived.Common.Simulations.Extensions.System;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelKey
    {
        public static ParallelKey Empty = new ParallelKey(default);

        private const int TypeAndGuidSize = 17;

        public readonly Guid Hash;

        internal ParallelKey(Guid hash)
        {
            this.Hash = hash;
        }

        public unsafe static ParallelKey NewKey()
        {
            return new ParallelKey(Guid.NewGuid());
        }

        public ParallelKey Create(ParallelType type)
        {
            return ParallelKey.From(type, this);
        }

        public ParallelKey Create(ParallelType type, params int[] noise)
        {
            return ParallelKey.From(type, this, noise);
        }

        public static ParallelKey From(ParallelType type, params int[] noise)
        {
            return ParallelKey.From(type, default, noise);
        }

        private static unsafe ParallelKey From(ParallelType type, ParallelKey parent, params int[] noise)
        {
            byte[] data = new byte[TypeAndGuidSize + (noise.Length * sizeof(int))];
            data[0] = type.Value;
            data.Encode(1, parent.Hash);
            Buffer.BlockCopy(noise, 0, data, TypeAndGuidSize, noise.Length * sizeof(int));
            uint128 xxHash = xxHash128.ComputeHash(data, data.Length);
            Guid* hash = (Guid*)&xxHash;

            return new ParallelKey(hash[0]);
        }
    }
}
