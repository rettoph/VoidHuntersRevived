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
using Autofac;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class InputEngine : BasicEngine,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IStepEngine<Tick>
    {
        private readonly ClientPeer _client;
        private readonly Camera2D _camera;
        private readonly ISimulationService _simulations;

        private IEntityService _entities;
        private ITractorBeamEmitterService _tractorBeamEmitters;
        private ISocketService _sockets;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public string name { get; } = nameof(InputEngine);

        public InputEngine(
            ClientPeer client, 
            Camera2D camera,
            ISimulationService simulations)
        {
            _client = client;
            _camera = camera;
            _simulations = simulations;

            _entities = null!;
            _tractorBeamEmitters = null!;
            _sockets = null!;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            var inputScope = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep).Scope;

            _entities = inputScope.Resolve<IEntityService>();
            _tractorBeamEmitters = inputScope.Resolve<ITractorBeamEmitterService>();
            _sockets = inputScope.Resolve<ISocketService>();
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            _simulations.Input(
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

                _simulations.Input(
                    sender: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = shipId.VhId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = true
                    });

                _simulations.Input(
                    sender: eventId,
                    data: new Input_TractorBeamEmitter_Select()
                    {
                        ShipVhId = shipId.VhId,
                        TargetVhId = targetNode.Id.VhId
                    });
            }
            else
            {
                ref Tactical tactical = ref _entities.QueryById<Tactical>(shipId);

                _simulations.Input(
                    sender: eventId,
                    data: new Input_TractorBeamEmitter_Deselect()
                    {
                        ShipVhId = shipId.VhId,
                        AttachToSocketVhId =_sockets.TryGetClosestOpenSocket(shipId, tactical.Target, out Socket socket)
                            ? socket.Id.VhId : default
                    });
            }
        }

        public void Step(in Tick _param)
        {
            if (_client.Users.Current is null)
            {
                return;
            }

            VhId shipVhId = _client.Users.Current.GetUserShipId();
            if(!_entities.TryGetId(shipVhId, out EntityId localShipId))
            {
                return;
            }

            ref Tactical tactical = ref _entities.QueryById<Tactical>(localShipId);
            if (tactical.Uses == 0)
            {
                return;
            }

            _simulations.Input(
                sender: _param.Hash,
                data: new Tactical_SetTarget()
                {
                    ShipVhId = shipVhId,
                    Value = (FixVector2)this.CurrentTargetPosition,
                    Snap = false
                });
        }
    }
}
