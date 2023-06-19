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
using VoidHuntersRevived.Game.Client.Messages;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Client)]
    internal class InputEngine : BasicEngine, 
        ISubscriber<SetHelmDirectionInput>,
        ISubscriber<SetTractorBeamEmitterActiveInput>
    {
        private ClientPeer _client;

        public InputEngine(ClientPeer client)
        {
            _client = client;
        }

        public void Process(in Guid messageId, in SetHelmDirectionInput message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            this.Simulation.Enqueue(
                eventId: new VhId(messageId),
                data: new SetHelmDirection()
                {
                    ShipId = _client.Users.Current.GetUserShipId(),
                    Which = message.Which,
                    Value = message.Value
                });
        }

        public void Process(in Guid messageId, in SetTractorBeamEmitterActiveInput message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            this.Simulation.Enqueue(
                eventId: new VhId(messageId),
                data: new SetTractorBeamEmitterActive()
                {
                    ShipId = _client.Users.Current.GetUserShipId(),
                    Value = message.Value
                });
        }
    }
}
