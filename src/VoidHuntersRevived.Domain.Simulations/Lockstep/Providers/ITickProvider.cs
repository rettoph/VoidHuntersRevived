using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Providers
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
