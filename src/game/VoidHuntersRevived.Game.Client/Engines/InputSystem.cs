using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Network.Common;
using Guppy.Game.Common.Systems;
using Guppy.Game.Graphics.Common;
using Guppy.Game.Input.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Svelto.ECS;
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
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Game.Client.Messages;
using VoidHuntersRevived.Game.Core.Events;

namespace VoidHuntersRevived.Game.Client.Systems
{
    public class InputSystem(
        IStrategy strategy,
        ICamera2D camera,
        INetScope<IStrategy> netScope
    ) : ISceneSystem,
        IInitializeSystem,
        IInputSubscriber<Input_Helm_SetDirection>,
        IInputSubscriber<Input_TractorBeamEmitter_SetActive>,
        IInputSubscriber<Input_Spam_Click>,
        ITickSystem
    {
        private bool _spamClick;

        private readonly ICamera2D _camera = camera;
        private readonly INetScope<IStrategy> _netScope = netScope;
        private readonly IStrategy _strategy = strategy;

        private IEntityQueryService _readEntityQueryService = null!;
        private ITractorBeamEmitterService _readTractorBeamEmitterService = null!;
        private INodeSocketService _readSocketService = null!;

        private Vector2 CurrentTargetPosition => this._camera.Unproject(Mouse.GetState().Position.ToVector2());

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize()
        {
            IStrategy readStrategy = this._strategy.Simulation.First(StrategyTypeEnum.Predictive, StrategyTypeEnum.Lockstep) ?? throw new NotImplementedException();

            this._readEntityQueryService = readStrategy.Resolve<IEntityQueryService>();
            this._readTractorBeamEmitterService = readStrategy.Resolve<ITractorBeamEmitterService>();
            this._readSocketService = readStrategy.Resolve<INodeSocketService>();
        }

        public void Process(in Guid messageId, Input_Helm_SetDirection message)
        {
            VhId sourceId = new(messageId);

            this.ForEachCurrentUserEntity((shipLocalId, shipGlobalId) =>
            {
                this._strategy.Simulation.Input(
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
                    if (this._readTractorBeamEmitterService.Query(tractorBeamEmitterLocalId, (FixVector2)this.CurrentTargetPosition, out Node targetNode) == false)
                    {
                        return;
                    }

                    EntityGlobalId targetNodeGlobalId = this._readEntityQueryService.GetGlobalId(targetNode.LocalId);

                    this._strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Tactical_SetTarget()
                        {
                            ShipGlobalId = tractorBeamEmitterGlobalId,
                            Value = (FixVector2)this.CurrentTargetPosition,
                            Snap = true
                        });

                    this._strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Input_TractorBeamEmitter_Select()
                        {
                            TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                            TargetNodeGlobalId = targetNodeGlobalId
                        });
                }
                else
                {
                    ref Tactical tactical = ref this._readEntityQueryService.QueryByLocalId<Tactical>(tractorBeamEmitterLocalId);
                    NodeSocketGlobalId? attachToSocketLocalId = this._readSocketService.TryGetClosestOpenNodeSocket(tractorBeamEmitterLocalId, tactical.Target, out NodeSocket nodeSocket)
                                ? this._readSocketService.GetGlobalId(nodeSocket.LocalId) : null;

                    this._strategy.Simulation.Input(
                        sourceId: sourceId,
                        data: new Input_TractorBeamEmitter_Deselect()
                        {
                            TractorBeamEmitterGlobalId = tractorBeamEmitterGlobalId,
                            AttachToNodeSocketGlobalId = attachToSocketLocalId
                        });
                }
            });
        }

        [SequenceGroup<TickSequenceGroupEnum>(TickSequenceGroupEnum.InputEvents)]
        public void Tick(Tick tick)
        {
            if (this._spamClick)
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
                ref Tactical tactical = ref this._readEntityQueryService.QueryByLocalId<Tactical>(shipLocalId);
                if (tactical.Uses == 0)
                {
                    return;
                }

                this._strategy.Simulation.Input(
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
            this._spamClick = message.Value;
        }

        private void ForEachCurrentUserEntity(Action<EntityLocalId, EntityGlobalId> input)
        {
            int currentUserId = this._netScope.Group.Peer.Users.Current.Id;
            ref var filter = ref this._readEntityQueryService.GetFilter<EntityLocalId, IUser>(currentUserId);
            foreach (var (indices, group) in filter)
            {
                var (localIds, globalIds, _) = this._readEntityQueryService.QueryEntities<EntityLocalId, EntityGlobalId>(group);

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