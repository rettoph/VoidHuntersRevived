using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Identity;
using Guppy.Network.Peers;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Common.Events;

namespace VoidHuntersRevived.Game.Client.Systems
{
    [AutoLoad]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class InputSystem : BasicSystem,
        ISubscriber<SetHelmDirectionInput>
    {
        private readonly ISimulationService _simulations;
        private ClientPeer _client;

        public InputSystem(
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
