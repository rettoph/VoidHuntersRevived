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
using Guppy.Common;
using Guppy.Network.Identity;
using VoidHuntersRevived.Game.Client.Messages;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Common;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Guppy.Game.MonoGame.Utilities.Cameras;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Game.Ships.Services;
using VoidHuntersRevived.Game.Ships.Events;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Services;
using Guppy.Game.Input;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    internal class InputEngine : BasicEngine,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IStepEngine<Tick>
    {
        private readonly IEntityService _entities;
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;
        private readonly ITractorBeamEmitterService _tractorBeamEmitters;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public string name { get; } = nameof(InputEngine);

        public InputEngine(
            IEntityService entities, 
            ClientPeer client, 
            Camera2D camera,
            ITractorBeamEmitterService tractorBeamEmitters)
        {
            _entities = entities;
            _client = client;
            _camera = camera;
            _tractorBeamEmitters = tractorBeamEmitters;
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            this.Simulation.Input(
                sender: new VhId(messageId),
                data: new Helm_SetDirection()
                {
                    ShipVhId = _client.Users.Current.GetUserShipId(),
                    Which = message.Which,
                    Value = message.Value
                });
        }

        public void Process(in Guid messageId, Input_TractorBeamEmitter_SetActive message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            VhId eventId = new VhId(messageId);
            EntityId shipId = _entities.GetId(_client.Users.Current.GetUserShipId());

            if (message.Value)
            {
                if (!_tractorBeamEmitters.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode))
                {
                    return;
                }

                this.Simulation.Input(
                    sender: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = shipId.VhId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = true
                    });

                this.Simulation.Input(
                    sender: eventId,
                    data: new Input_TractorBeamEmitter_Select()
                    {
                        ShipVhId = shipId.VhId,
                        TargetVhId = targetNode.Id.VhId
                    });
            }
            else
            {
                this.Simulation.Input(
                    sender: eventId,
                    data: new Input_TractorBeamEmitter_Deselect()
                    {
                        ShipVhId = shipId.VhId
                    });
            }
        }

        public void Step(in Tick _param)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            VhId localShipVhId = _client.Users.Current.GetUserShipId();
            if(!_entities.TryGetId(localShipVhId, out EntityId localShipId))
            {
                return;
            }

            ref Tactical tactical = ref _entities.QueryById<Tactical>(localShipId);
            if (tactical.Uses == 0)
            {
                return;
            }

            this.Simulation.Input(
                sender: _param.Hash,
                data: new Tactical_SetTarget()
                {
                    ShipVhId = localShipVhId,
                    Value = (FixVector2)this.CurrentTargetPosition,
                    Snap = false
                });
        }
    }
}
