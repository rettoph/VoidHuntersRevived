using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Common;

namespace VoidHuntersRevived.Library.Simulations.Lockstep.Providers
{
    public interface ITickProvider
    {
        /// <summary>
        /// The highest tick ready and available to
        /// be processed.
        /// </summary>
        int AvailableId { get; }

        bool TryGetNextTick([MaybeNullWhen(false)] out Tick next);
    }
}
