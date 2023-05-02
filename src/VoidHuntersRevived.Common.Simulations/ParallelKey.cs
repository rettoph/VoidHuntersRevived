using Standart.Hash.xxHash;
using VoidHuntersRevived.Common.Simulations.Extensions.System;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelKey
    {
        public static ParallelKey Empty = new ParallelKey(default);

        public readonly UInt128 Value;

        internal unsafe ParallelKey(UInt128 value)
        {
            this.Value = value;
        }

        public ParallelKey Next()
        {
            return new ParallelKey(this.Value + 1);
        }

        public ParallelKey Previous()
        {
            return new ParallelKey(this.Value - 1);
        }

        public unsafe static ParallelKey NewKey()
        {
            Guid valueGuid = Guid.NewGuid();
            UInt128* value = (UInt128*)&valueGuid;

            return new ParallelKey(value[0]);
        }
    }
}
