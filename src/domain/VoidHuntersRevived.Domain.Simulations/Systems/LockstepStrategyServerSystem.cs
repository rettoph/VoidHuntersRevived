using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Core.Messaging.Common;
using Guppy.Core.Messaging.Common.Enums;
using Guppy.Core.Network.Common;
using Guppy.Core.Network.Common.Extensions;
using Guppy.Core.Network.Common.Identity.Enums;
using Guppy.Core.Network.Common.Services;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Events;
using VoidHuntersRevived.Domain.Simulations.Common.Extensions;
using VoidHuntersRevived.Domain.Simulations.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    /// <summary>
    /// System implementing server specific logic for <see cref="Strategies.LockstepStrategy"/>
    /// This includes:
    ///     - Publishing <see cref="SimulationBegin"/> input, kickstarting the simulation
    ///     - Publishing <see cref="UserJoined"/> input when a new user connects to the current <see cref="INetScope{IStrategy}"/>
    ///     - Broadcasting Tick data to all connected users every tick
    ///     - Broadcasting tick history to new users on <see cref="UserJoined"/> inputs
    /// </summary>
    /// <param name="netScope"></param>
    /// <param name="tickService"></param>
    /// <param name="eventService"></param>
    public class LockstepStrategyServerSystem(
        INetScope<IStrategy> netScope,
        ITickService tickService,
        IStepEventService eventService
    ) : ISceneSystem,
        IInitializeSystem,
        IDeinitializeSystem,
        ITickSystem,
        IEventSystem<UserJoined>,
        INetIncomingMessageSubscriber<EnqueuedStepInput>,
        ISubscriber<SubscriberSequenceGroupEnum, EnqueuedStepInput>
    {
        private readonly INetScope _netScope = netScope;
        private readonly ITickService _tickService = tickService;
        private readonly IStepEventService _eventService = eventService;
        private readonly List<Tick> _history = [];

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PostInitialize)]
        public void Initialize()
        {
            // Subscribe to netScope user joined event
            // The handler will create and publish a UserJoined input event
            this._netScope.Group.Users.OnUserJoined += this.HandleUserJoined;

            // Kick off the simulation with a SimulationBegin message
            this._eventService.Input(NameSpace<LockstepStrategyServerSystem>.Instance, new SimulationBegin());
        }

        [SequenceGroup<DeinitializeSequenceGroupEnum>(DeinitializeSequenceGroupEnum.PostInitialize)]
        public void Deinitialize()
        {
            this._netScope.Group.Users.OnUserJoined -= this.HandleUserJoined;
        }

        /// <summary>
        /// Every tick we will broadcast the entire tick data to all
        /// currently connected users.
        /// </summary>
        /// <param name="tick"></param>
        [SequenceGroup<TickSequenceGroupEnum>(TickSequenceGroupEnum.PublishInputs)]
        public void Tick(Tick tick)
        {
            // Broadcast the current tick to all connected peers
            this._netScope.CreateMessage(in tick)
                .AddRecipients(this._netScope.Group.Users.Peers);

            if (tick.Inputs.Length == 0)
            {
                return;
            }

            // If there is any input data this tick we can store it so that it can be
            // re-sent to any newly connected users who might join mid game
            this._history.Add(tick);
        }

        /// <summary>
        /// When a new user joins we must send a <see cref="TickHistoryStart"/> message to them
        /// alerting them of incoming <see cref="Common.Tick"/> data.
        /// 
        /// Then we send them several <see cref="TickHistoryItem"/> messages containing relevent tick data.
        /// 
        /// Finally we will sent them <see cref="TickHistoryEnd"/> message - alerting the new user that
        /// they have been caugh up with the live simulation.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        [SequenceGroup<EventSequenceGroupEnum>(EventSequenceGroupEnum.Process)]
        public void Process(in VhId id, UserJoined data)
        {
            IUser? user = this._netScope.Group.Peer!.Users.UpdateOrCreate(data.UserDto);

            if (user.NetPeer is null)
            {
                return;
            }

            int currentTickId = this._tickService.LastTickId;

            this._netScope.CreateMessage(new TickHistoryStart()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);

            foreach (Tick tick in this._history)
            {
                if (tick.Id > currentTickId)
                {
                    break;
                }

                this._netScope.CreateMessage(new TickHistoryItem()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer);
            }

            this._netScope.CreateMessage(new TickHistoryEnd()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer);
        }

        /// <summary>
        /// When the server recieves an <see cref="EnqueuedStepInput"/> we must alert the
        /// strategy. 
        /// 
        /// TODO: this is where user input verification will someday happen
        /// </summary>
        /// <param name="message"></param>
        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(INetIncomingMessage<EnqueuedStepInput> message)
        {
            this._eventService.Input(message.Body);
        }

        /// <summary>
        /// ???
        /// Not sure why this is here. It never seems to be called.
        /// This was noticed on 2/17/2025. If this exception is still not being
        /// thrown in the distant future its probably safe to remove.
        /// </summary>
        /// <param name="message"></param>
        [SequenceGroup<SubscriberSequenceGroupEnum>(SubscriberSequenceGroupEnum.Process)]
        public void Process(EnqueuedStepInput message)
        {
            this._eventService.Input(message);
            throw new NotImplementedException();
        }

        /// <summary>
        /// When a new user connects we must publish a <see cref="UserJoined"/> input
        /// message to the strategy.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void HandleUserJoined(INetScopeUserService sender, IUser args)
        {
            this._eventService.Input(VhId.NewVhId(), new UserJoined()
            {
                UserDto = args.ToDto(ClaimAccessibilityEnum.Public)
            });
        }
    }
}
