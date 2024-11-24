using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common.Attributes;
using Guppy.Core.Network.Common.Enums;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Input.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
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
    [PeerFilter(PeerType.Client)]
    [StrategyFilter(StrategyTypeEnum.Lockstep)]
    internal class InputEngine(
        ICamera2D camera
    ) : StrategyEngine,
        IOnInitializeEngine<IStrategy>,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IInputSubscriber<Input_Spam_Click>,
        IOnTickEngine
    {
        private bool _spamClick;

        private readonly ICamera2D _camera = camera;

        private IEntityQueryService _readEntityQueryService = null!;
        private ITractorBeamEmitterService _readTractorBeamEmitterService = null!;
        private ISocketService _readSocketService = null!;
        private IUserShipService _readUserShipService = null!;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            IStrategy readStrategy = this.Strategy.Simulation.First(StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep) ?? throw new NotImplementedException();

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

            this.Strategy.Simulation.Input(
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

            VhId eventId = new(messageId);

            if (message.Value)
            {
                if (_readTractorBeamEmitterService.Query(shipId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
                {
                    return;
                }

                this.Strategy.Simulation.Input(
                    sourceId: eventId,
                    data: new Tactical_SetTarget()
                    {
                        ShipVhId = shipId.VhId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = true
                    });

                this.Strategy.Simulation.Input(
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

                this.Strategy.Simulation.Input(
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

            this.Strategy.Simulation.Input(
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
