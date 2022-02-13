using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Components.Ships;
using VoidHuntersRevived.Library.Messages.Requests;

namespace VoidHuntersRevived.Library.MessageProcessors
{
    public sealed class DirectionRequestProcessor : ConsolidationProcessor<DirectionRequest, DirectionComponent>
    {
        public override Int32 MessagePriority => Globals.Constants.MessageQueues.DirectionRequestQueue;
    }
}
