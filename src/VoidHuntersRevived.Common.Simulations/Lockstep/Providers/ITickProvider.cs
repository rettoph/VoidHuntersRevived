using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Simulations.Lockstep.Providers
{
    public interface ITickProvider
    {
        void Enqueue(Tick tick);
        bool TryDequeueNext([MaybeNullWhen(false)] out Tick tick);
        bool PeekHead([MaybeNullWhen(false)] out Tick tick);
        bool PeekTail([MaybeNullWhen(false)] out Tick tick);
        void Reset();
    }
}
