using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations.Systems;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Events;
using VoidHuntersRevived.Domain.Simulations.Messages;

namespace VoidHuntersRevived.Domain.Simulations.Systems.Lockstep
{
    [AutoLoad]
    [PeerTypeFilter(PeerType.Server)]
    [SimulationTypeFilter(SimulationType.Lockstep)]
    internal sealed class LockstepServer_TickSystem : BasicSystem,
        ITickSystem,
        IEventSubscriber<UserJoined>
    {
        private readonly NetScope _scope;
        private readonly List<Tick> _history;

        public LockstepServer_TickSystem(NetScope scope)
        {
            _scope = scope;
            _history = new List<Tick>();
        }

        public void Process(in EventId id, UserJoined data)
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

        public void Tick(Tick tick)
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
