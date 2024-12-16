using Guppy.Core.Common.Attributes;
using Guppy.Core.Network.Common;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Input.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Events;
using VoidHuntersRevived.Domain.Ships.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Engines
{
    internal class InputEngine(
        ICamera2D camera,
        INetScope<IStrategy> netScope
    ) : StrategyEngine<ILockstepStrategy>,
        IClientEngine,
        IOnInitializeEngine<IStrategy>,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IInputSubscriber<Input_Spam_Click>,
        IOnTickEngine
    {
        private bool _spamClick;

        private readonly ICamera2D _camera = camera;
        private readonly INetScope<IStrategy> _netScope = netScope;

        private IEntityQueryService _readEntityQueryService = null!;
        private ITractorBeamEmitterService _readTractorBeamEmitterService = null!;
        private INodeSocketService _readSocketService = null!;

        private Vector2 CurrentTargetPosition => _camera.Unproject(Mouse.GetState().Position.ToVector2());

        [SequenceGroup<OnInitializeSequenceGroup>(OnInitializeSequenceGroup.Initialize)]
        public void OnInitialize(IStrategy strategy)
        {
            IStrategy readStrategy = this.Strategy.Simulation.First(StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep) ?? throw new NotImplementedException();

            _readEntityQueryService = readStrategy.Engines.Get<IEntityService>().Query;
            _readTractorBeamEmitterService = readStrategy.Engines.Get<ITractorBeamEmitterService>();
            _readSocketService = readStrategy.Engines.Get<INodeSocketService>();
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            VhId sourceId = new(messageId);

            this.ForEachCurrentUserEntity((shipLocalId, shipGlobalId) =>
            {
                this.Strategy.Simulation.Input(
                    sourceId: sourceId,
                    data: new Helm_SetDirection()
                    {
                        ShipGlobalId = shipGlobalId,
                        Which = message.Which,
                        Value = message.Value
                    });
            });
        }

        public void Process(in Guid messageId, Input_TractorBeamEmitter_SetActive message)
        {
            VhId sourceId = new(messageId);

            this.ForEachCurrentUserEntity((tractorBeamEmitterLocalId, tractorBeamEmitterGlobalId) =>
            {
                if (message.Value)
                {
                    if (_readTractorBeamEmitterService.Query(tractorBeamEmitterLocalId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
                    {
                        return;
                    }

                    this.Strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Tactical_SetTarget()
                        {
                            ShipGlobalId = tractorBeamEmitterGlobalId,
                            Value = (FixVector2)this.CurrentTargetPosition,
                            Snap = true
                        });

                    this.Strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Input_TractorBeamEmitter_Select()
                        {
                            TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                            TargetNodeGlobalId = targetNode.Id.ToGlobalEntityId()
                        });
                }
                else
                {
                    ref Tactical tactical = ref _readEntityQueryService.QueryByLocalId<Tactical>(tractorBeamEmitterLocalId);
                    NodeSocketGlobalId? attachToSocketLocalId = _readSocketService.TryGetClosestOpenNodeSocket(new EntityId(tractorBeamEmitterLocalId.Value, tractorBeamEmitterGlobalId.Value), tactical.Target, out NodeSocket nodeSocket)
                                ? _readSocketService.GetGlobalId(nodeSocket.LocalId) : null;

                    this.Strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Input_TractorBeamEmitter_Deselect()
                        {
                            TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                            AttachToSocketVhId = attachToSocketLocalId
                        });
                }
            });
        }

        [SequenceGroup<OnTickSequenceGroup>(OnTickSequenceGroup.InputEvents)]
        public void OnTick(Tick tick)
        {
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

            this.ForEachCurrentUserEntity((shipLocalId, shipGlobalId) =>
            {
                ref Tactical tactical = ref _readEntityQueryService.QueryByLocalId<Tactical>(shipLocalId);
                if (tactical.Uses == 0)
                {
                    return;
                }

                this.Strategy.Simulation.Input(
                    sourceId: tick.Hash,
                    data: new Tactical_SetTarget()
                    {
                        ShipGlobalId = shipGlobalId,
                        Value = (FixVector2)this.CurrentTargetPosition,
                        Snap = false
                    });
            });
        }

        public void Process(in Guid messageId, Input_Spam_Click message)
        {
            _spamClick = message.Value;
        }

        private void ForEachCurrentUserEntity(Action<EntityLocalId, EntityGlobalId> input)
        {
            int currentUserId = _netScope.Group.Peer.Users.Current.Id;
            ref var filter = ref _readEntityQueryService.GetFilter<EntityLocalId, IUser>(currentUserId);
            foreach (var (indices, group) in filter)
            {
                var (localIds, globalIds, _) = _readEntityQueryService.QueryEntities<EntityLocalId, EntityGlobalId>(group);

                for (int i = 0; i < indices.count; i++)
                {
                    EntityLocalId localId = localIds[indices[i]];
                    EntityGlobalId globalId = globalIds[indices[i]];
                    input(localId, globalId);
                }
            }
        }
    }
}
