using Guppy.EntityComponent;
using Guppy.Threading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Messages.Requests;

namespace VoidHuntersRevived.Library.MessageProcessors
{
    /// <summary>
    /// Helper service useful for processing tractor beam related messages.
    /// </summary>
    public sealed class TractorBeamRequestProcessor : ConsolidationProcessor<TractorBeamStateRequest, TractorBeamComponent>
    {
        public override int MessagePriority => Globals.Constants.MessageQueues.TractorBeamRequestQueue;
    }
}
