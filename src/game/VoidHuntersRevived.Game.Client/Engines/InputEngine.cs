using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Input.Common;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [PeerFilter(PeerType.Client)]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    internal class InputEngine : StrategyEngine,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IInputSubscriber<Input_Spam_Click>,
        IOnTickEngine
    {
        private bool _spamClick;

        private readonly Camera2D _camera;
        private readonly ISimulation _simulation;

        private IEntityQueryService _readEntityQueryService;
        private ITractorBeamEmitterService _readTractorBeamEmitterService;
        private ISocketService _readSocketService;
        private IUserShipService _readUserShipService;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        public string name { get; } = nameof(InputEngine);

        public InputEngine(
            Camera2D camera,
            IEntityQueryService entityQueryService,
            ISimulation simulation)
        {
            _camera = camera;
            _simulation = simulation;

            _readEntityQueryService = null!;
            _readTractorBeamEmitterService = null!;
            _readSocketService = null!;
            _readUserShipService = null!;
        }

        public override void Initialize(IStrategy strategy)
        {
            base.Initialize(strategy);

            IStrategy readStrategy = _simulation.First(StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep) ?? throw new NotImplementedException();

            _readEntityQueryService = readStrategy.Engines.Get<IEntityService>().Query;
            _readTractorBeamEmitterService = readStrategy.Engines.Get<ITractorBeamEmitterService>();
            _readSocketService = readStrategy.Engines.Get<ISocketService>();
            _readUserShipService = readStrategy.Engines.Get<IUserShipService>();
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            if (_readUserShipService.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            _simulation.Input(
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
            if (_readUserShipService.TryGetCurrentUserShipId(out EntityId shipId) == false)
            {
                return;
            }

            VhId eventId = new VhId(messageId);

            if (message.Value)
            {
                if (_readTractorBeamEmitterService.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
                {
                    return;
                }

                _simulation.Input(
                    sourceId: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = shipId.VhId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = true
                    });

                _simulation.Input(
                    sourceId: eventId,
                    data: new Input_TractorBeamEmitter_Select()
                    {
                        ShipVhId = shipId.VhId,
                        TargetVhId = targetNode.Id.VhId
                    });
            }
            else
            {
                ref Tactical tactical = ref _readEntityQueryService.QueryById<Tactical>(shipId);
                SocketVhId? attachToSocket = _readSocketService.TryGetClosestOpenSocket(shipId, tactical.Target, out NodeSocket nodeSocket)
                            ? nodeSocket.Id.VhId : null;

                _simulation.Input(
                    sourceId: eventId,
                    data: new Input_TractorBeamEmitter_Deselect()
                    {
                        ShipVhId = shipId.VhId,
                        AttachToSocketVhId = attachToSocket
                    });
            }
        }

        [SequenceGroup<OnTickSequenceGroup>(OnTickSequenceGroup.InputEvents)]
        public void OnTick(Tick tick)
        {
            if (_readUserShipService.TryGetCurrentUserShipId(out EntityId shipId) == false)
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

            ref Tactical tactical = ref _readEntityQueryService.QueryById<Tactical>(shipId);
            if (tactical.Uses == 0)
            {
                return;
            }

            _simulation.Input(
                sourceId: tick.Hash,
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
