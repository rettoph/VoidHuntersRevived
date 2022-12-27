using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Simulations.EventTypes;

namespace VoidHuntersRevived.Library.Providers
{
    public interface ITickProvider
    {
        /// <summary>
        /// Should represent the highest tick ready and available to
        /// be processed.
        /// </summary>
        int AvailableId { get; }

        TickProviderStatus Status { get; }

        bool TryGetNextTick([MaybeNullWhen(false)] out Tick next);
    }
}
