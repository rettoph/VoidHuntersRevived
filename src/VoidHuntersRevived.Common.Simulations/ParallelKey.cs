using Standart.Hash.xxHash;
using VoidHuntersRevived.Common.Simulations.Extensions.System;

namespace VoidHuntersRevived.Common.Simulations
{
    public readonly struct ParallelKey
    {
        public static ParallelKey Empty = new ParallelKey(default);

        public readonly Guid Value;

        internal ParallelKey(Guid value)
        {
            this.Value = value;
        }

        public unsafe static ParallelKey NewKey()
        {
            return new ParallelKey(Guid.NewGuid());
        }
    }
}
