using Autofac;
using Guppy.Attributes;
using Guppy.Game.Input;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class InputEngine : BasicEngine,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IInputSubscriber<Input_Spam_Click>,
        IStepEngine<Tick>
    {
        private bool _spamClick;

        private readonly Camera2D _camera;
        private readonly ISimulationService _simulations;

        private IEntityService _entities;
        private ITractorBeamEmitterService _tractorBeamEmitters;
        private ISocketService _sockets;
        private IUserShipService _userShips;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public string name { get; } = nameof(InputEngine);

        public InputEngine(
            Camera2D camera,
            ISimulationService simulations)
        {
            _camera = camera;
            _simulations = simulations;

            _entities = null!;
            _tractorBeamEmitters = null!;
            _sockets = null!;
            _userShips = null!;
        }

        public override void Initialize(ISimulation simulation)
        {
            base.Initialize(simulation);

            var inputScope = _simulations.First(SimulationType.Predictive, SimulationType.Lockstep).Scope;

            _entities = inputScope.Resolve<IEntityService>();
            _tractorBeamEmitters = inputScope.Resolve<ITractorBeamEmitterService>();
            _sockets = inputScope.Resolve<ISocketService>();
            _userShips = inputScope.Resolve<IUserShipService>();
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            if (_userShips.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            _simulations.Input(
                sourceId: new VhId(messageId),
                data: new Helm_SetDirection()
                {
                    ShipVhId = shipId.VhId,
                    Which = message.Which,
                    Value = message.Value
                });
        }

        public void Process(in Guid messageId, Input_TractorBeamEmitter_SetActive message)
        {
            if (_userShips.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            VhId eventId = new VhId(messageId);

            if (message.Value)
            {
                if (_tractorBeamEmitters.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
                {
                    return;
                }

                _simulations.Input(
                    sourceId: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = shipId.VhId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = true
                    });

                _simulations.Input(
                    sourceId: eventId,
                    data: new Input_TractorBeamEmitter_Select()
                    {
                        ShipVhId = shipId.VhId,
                        TargetVhId = targetNode.Id.VhId
                    });
            }
            else
            {
                ref Tactical tactical = ref _entities.QueryById<Tactical>(shipId);
                SocketVhId? attachToSocket = _sockets.TryGetClosestOpenSocket(shipId, tactical.Target, out Socket socket)
                            ? socket.Id.VhId : null;

                _simulations.Input(
                    sourceId: eventId,
                    data: new Input_TractorBeamEmitter_Deselect()
                    {
                        ShipVhId = shipId.VhId,
                        AttachToSocketVhId = attachToSocket
                    });
            }
        }

        public void Step(in Tick _param)
        {
            if (_userShips.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            if (_spamClick)
            {
                this.Process(Guid.NewGuid(), new Input_TractorBeamEmitter_SetActive(true));
                this.Process(Guid.NewGuid(), new Input_TractorBeamEmitter_SetActive(false));
                int count = Random.Shared.Next(1, 5);
                for (int i = 0; i < count; i++)
                {
                    this.Process(Guid.NewGuid(), new Input_TractorBeamEmitter_SetActive(true));
                    this.Process(Guid.NewGuid(), new Input_TractorBeamEmitter_SetActive(false));
                }
            }

            ref Tactical tactical = ref _entities.QueryById<Tactical>(shipId);
            if (tactical.Uses == 0)
            {
                return;
            }

            _simulations.Input(
                sourceId: _param.Hash,
                data: new Tactical_SetTarget()
                {
                    ShipVhId = shipId.VhId,
                    Value = (FixVector2)this.CurrentTargetPosition,
                    Snap = false
                });
        }

        public void Process(in Guid messageId, Input_Spam_Click message)
        {
            _spamClick = message.Value;
        }
    }
}
