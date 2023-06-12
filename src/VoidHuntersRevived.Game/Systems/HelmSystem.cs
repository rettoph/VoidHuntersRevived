using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Common.Events;

namespace VoidHuntersRevived.Game.Systems
{
    [AutoLoad]
    internal sealed class HelmSystem : BasicSystem,
        IEventSubscriber<SetHelmDirection>
    {
        public void Process(in EventId id, SetHelmDirection data)
        {
            throw new NotImplementedException();
        }
    }
}
