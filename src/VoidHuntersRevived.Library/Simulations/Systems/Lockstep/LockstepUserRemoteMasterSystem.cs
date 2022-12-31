using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Maps;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Simulations.EventData;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class LockstepUserRemoteMasterSystem : ISystem, ILockstepSimulationSystem,
        ISubscriber<INetIncomingMessage<DirectionInput>>
    {
        private readonly ISimulationService _simulations;
        private readonly ITickFactory _tickFactory;

        public LockstepUserRemoteMasterSystem(ISimulationService simulations, ITickFactory tickFactory)
        {
            _tickFactory = tickFactory;
            _simulations = simulations;
        }

        public void Initialize(World world)
        {
            //
        }

        public void Dispose()
        {
            // 
        }

        public void Process(in INetIncomingMessage<DirectionInput> message)
        {
            if (message.Peer is null)
            {
                return;
            }

            _tickFactory.Enqueue(new PilotDirectionInput(
                pilotId: _simulations.UserIdMap.Get(message.Peer.Id),
                which: message.Body.Which,
                value: message.Body.Value));
        }
    }
}
