﻿using Guppy.Attributes;
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
using VoidHuntersRevived.Game.Services;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using Svelto.ECS;
using VoidHuntersRevived.Common.Simulations.Lockstep;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Client)]
    internal class InputEngine : BasicEngine, 
        ISubscriber<Input_Helm_SetDirection>,
        ISubscriber<Input_TractorBeamEmitter_SetActive>,
        IStepEngine<Tick>
    {
        private readonly IEntityService _entities;
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;
        private readonly TractorBeamEmitterService _tractorBeamEmitterService;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public string name { get; } = nameof(InputEngine);

        public InputEngine(
            IEntityService entities, 
            ClientPeer client, 
            Camera2D camera, 
            TractorBeamEmitterService tractorBeamEmitterService)
        {
            _entities = entities;
            _client = client;
            _camera = camera;
            _tractorBeamEmitterService = tractorBeamEmitterService;
        }

        public void Process(in Guid messageId, in Input_Helm_SetDirection message)
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

        public void Process(in Guid messageId, in Input_TractorBeamEmitter_SetActive message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            VhId eventId = new VhId(messageId);
            IdMap shipId = _entities.GetIdMap(_client.Users.Current.GetUserShipId());

            if (message.Value)
            {
                if (!_tractorBeamEmitterService.Query(shipId, out IdMap targetId))
                {
                    return;
                }

                this.Simulation.Input(
                    sender: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = _client.Users.Current.GetUserShipId(),
                        Value = (FixVector2)this.CurrentTargetPosition
                    });

                this.Simulation.Input(
                    sender: eventId,
                    data: new TractorBeamEmitter_TryActivate()
                    {
                        ShipVhId = shipId.VhId,
                        TargetVhId = targetId.VhId
                    });
            }
            else
            {
                this.Simulation.Input(
                    sender: eventId,
                    data: new TractorBeamEmitter_TryDeactivate()
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

            this.Simulation.Input(
                sender: NameSpace<Tick>.Instance.Create(_param.Id),
                data: new Tactical_SetTarget()
                {
                    ShipVhId = _client.Users.Current.GetUserShipId(),
                    Value = (FixVector2)this.CurrentTargetPosition
                });
        }
    }
}
