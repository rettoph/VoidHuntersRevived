using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Identity;
using Guppy.Network.Messages;
using Guppy.Network;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Messages;
using Guppy.Network.Extensions.Identity;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Services;
using MonoGame.Extended.Entities;
using Guppy.Attributes;
using Guppy.Network.Enums;
using VoidHuntersRevived.Library.Attributes;
using LiteNetLib;
using Serilog;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class LockstepUserPilotRemoteMasterSystem : ISystem, ILockstepSimulationSystem
    {
        private readonly NetScope _netScope;
        private readonly ITickFactory _tickFactory;
        private readonly SimulationState _state;
        private readonly ILogger _log;
        private ushort _id = 1;

        public LockstepUserPilotRemoteMasterSystem(
            NetScope netScope,
            SimulationState state,
            ITickFactory tickFactory,
            ILogger log)
        {
            _netScope = netScope;
            _state = state;
            _tickFactory = tickFactory;
            _log = log;
        }

        public void Initialize(World world)
        {
            _netScope.Users.OnUserJoined += HandleUserJoined;
        }

        public void Dispose()
        {
            _netScope.Users.OnUserJoined -= HandleUserJoined;
        }

        private void HandleUserJoined(IUserService sender, User newUser)
        {
            if (newUser.NetPeer is null)
            {
                return;
            }

            // Enqueue a new user joined action for the new user.
            _tickFactory.Enqueue(new UserPilot(
                pilotId: _id++,
                user: newUser.CreateAction(
                    action: UserAction.Actions.UserJoined,
                    accessibility: ClaimAccessibility.Public)));

            var lastTickId = _state.LastTickId;

            _log.Verbose($"New User Connected - Sending GameState at TickId: {lastTickId}");

            for (var i = 0; i < _state.History.Count; i++)
            {
                var tick = _state.History[i];

                if (tick.Id > lastTickId)
                {
                    break;
                }

                _log.Verbose($"Sending TickId: {tick.Id}");

                var om = _netScope.Create(new SimulationStateTick(tick))
                    .AddRecipient(newUser.NetPeer)
                    .Enqueue();
            }

            _log.Verbose($"Sending GameStateEnd: {lastTickId}");
            _netScope.Create(new SimulationStateEnd(lastTickId))
                .AddRecipient(newUser.NetPeer)
                .Enqueue();
        }
    }
}
