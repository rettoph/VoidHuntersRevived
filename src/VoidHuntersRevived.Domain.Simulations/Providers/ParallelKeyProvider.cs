using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Providers;

namespace VoidHuntersRevived.Domain.Simulations.Providers
{
    internal sealed class ParallelKeyProvider : IParallelKeyProvider
    {
        private Dictionary<ParallelKey, ushort> _counts = new Dictionary<ParallelKey, ushort>();

        public ParallelKey Next(in ParallelKey key)
        {
            ref ushort count = ref CollectionsMarshal.GetValueRefOrAddDefault(_counts, key, out _);

            return key.Step(count++);
        }

        public ParallelKey Previous(in ParallelKey key)
        {
            ref ushort count = ref CollectionsMarshal.GetValueRefOrAddDefault(_counts, key, out _);

            return key.Step(count--);
        }
    }
}
