using Guppy.Common;
using Guppy.Resources.Providers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Lockstep.Factories;
using VoidHuntersRevived.Common.Simulations.Lockstep.Messages;
using VoidHuntersRevived.Common.Simulations.Lockstep.Providers;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Constants;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep
{
    [PeerTypeFilter(PeerType.Client)]
    internal class StateClient : StateDefault
    {
        private NetScope _network;


        public StateClient(
            NetScope network,
            IBus bus,
            IFiltered<ITickProvider> ticks,
            ISettingProvider settings,
            ILogger log) : base(bus, ticks,settings, log)
        {
            _network = network;
        }

        public override void Enqueue(SimulationEventData input)
        {
            // Create a event data request message and broadcast to the server
            _network.Messages.Create(input).Enqueue();
        }
    }
}
