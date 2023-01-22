﻿using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using VoidHuntersRevived.Common.Entities.Extensions;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Events;
using VoidHuntersRevived.Domain.Simulations.Lockstep.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Lockstep.Systems
{
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class UserServerSystem : BasicSystem, ISubscriber<IInput<UserJoined>>
    {
        private readonly State _state;
        private readonly NetScope _scope;
        private readonly ISimulationService _simulations;

        public UserServerSystem(State state, NetScope scope, ISimulationService simulations)
        {
            _state = state;
            _scope = scope;
            _simulations = simulations;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _scope.Users.OnUserJoined += this.HandleUserJoined;
        }

        public void Process(in IInput<UserJoined> message)
        {
            var user = _scope.Peer!.Users.UpdateOrCreate(message.Data.Id, message.Data.Claims);

            if(user.NetPeer is null)
            {
                return;
            }

            var lastTickId = _state.LastTickId;

            _scope.Messages.Create<StateBegin>(new StateBegin())
                .AddRecipient(user.NetPeer)
                .Enqueue();

            foreach (Tick tick in _state.History)
            {
                if (tick.Id > lastTickId)
                {
                    break;
                }

                _scope.Messages.Create<StateTick>(new StateTick()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer).Enqueue();
            }

            _scope.Messages.Create<StateEnd>(new StateEnd()
            {
                LastTickId = lastTickId
            }).AddRecipient(user.NetPeer).Enqueue();
        }

        private void HandleUserJoined(IUserService sender, User args)
        {
            _simulations[SimulationType.Lockstep].Input(
                user: ParallelKeys.System,
                data: new UserJoined()
                {
                    Id = args.Id,
                    Claims = args.Where(x => x.Accessibility == ClaimAccessibility.Public).ToArray()
                });
        }
    }
}
