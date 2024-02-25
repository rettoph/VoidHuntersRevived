﻿using Guppy.Attributes;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Serilog;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Engines.Lockstep
{
    [AutoLoad]
    [PeerFilter(PeerType.Server)]
    [SimulationFilter(SimulationType.Lockstep)]
    internal class LockstepServer_TickEngine : BasicEngine<ILockstepSimulation>,
        ITickEngine,
        IEventEngine<UserJoined>
    {
        private readonly INetScope _scope;
        private readonly List<Tick> _history;
        private readonly ILogger _logger;

        public LockstepServer_TickEngine(ILogger logger, INetScope scope)
        {
            _scope = scope;
            _history = new List<Tick>();
            _logger = logger;
        }

        public string name { get; } = nameof(LockstepServer_TickEngine);

        public void Process(VhId id, UserJoined data)
        {
            User? user = _scope.Peer!.Users.UpdateOrCreate(data.UserId, data.Claims);

            if (user.NetPeer is null)
            {
                return;
            }

            var currentTickId = this.Simulation.CurrentTick.Id;

            _scope.Messages.Create(new TickHistoryStart()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer).Enqueue();

            foreach (Tick tick in _history)
            {
                if (tick.Id > currentTickId)
                {
                    break;
                }

                _scope.Messages.Create(new TickHistoryItem()
                {
                    Tick = tick
                }).AddRecipient(user.NetPeer).Enqueue();
            }

            _scope.Messages.Create(new TickHistoryEnd()
            {
                CurrentTickId = currentTickId
            }).AddRecipient(user.NetPeer).Enqueue();
        }

        public void Step(in Tick tick)
        {
            // Broadcast the current tick to all connected peers
            _scope.Messages.Create(in tick)
                .AddRecipients(_scope.Users.Peers)
                .Enqueue();

            if (tick.Events.Length == 0)
            {
                return;
            }

            _history.Add(tick);
        }
    }
}
