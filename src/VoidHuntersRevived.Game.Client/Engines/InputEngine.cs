using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using Guppy.Network.Peers;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Game.Common.Events;
using VoidHuntersRevived.Game.Events;
using Guppy.Common;
using Guppy.Network.Identity;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal class InputEngine : BasicEngine, 
        ISubscriber<SetHelmDirectionInput>
    {
        private readonly ISimulationService _simulations;
        private ClientPeer _client;

        public InputEngine(
            ClientPeer client,
            ISimulationService simulations)
        {
            _client = client;
            _simulations = simulations;
        }

        public void Process(in SetHelmDirectionInput message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            _simulations.Enqueue(new SetHelmDirection()
            {
                ShipId = _client.Users.Current.GetUserShipId(),
                Which = message.Which,
                Value = message.Value
            });
        }
    }
}
