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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Guppy.MonoGame.Utilities.Cameras;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Client)]
    internal class InputEngine : BasicEngine, 
        ISubscriber<SetHelmDirectionInput>,
        ISubscriber<SetTractorBeamEmitterActiveInput>
    {
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public InputEngine(ClientPeer client, Camera2D camera)
        {
            _client = client;
            _camera = camera;
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

            VhId eventId = new VhId(messageId);

            this.Simulation.Enqueue(
                eventId: eventId.Create(1),
                data: new SetTacticalTarget()
                {
                    ShipId = _client.Users.Current.GetUserShipId(),
                    Value = (FixVector2)this.CurrentTargetPosition
                });

            this.Simulation.Enqueue(
                eventId: eventId.Create(2),
                data: new SetTractorBeamEmitterActive()
                {
                    ShipId = _client.Users.Current.GetUserShipId(),
                    Value = message.Value
                });
        }
    }
}
