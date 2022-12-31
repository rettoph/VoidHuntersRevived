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
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Simulations.EventData;
using VoidHuntersRevived.Library.Simulations.EventData.Inputs;

namespace VoidHuntersRevived.Library.Simulations.Systems.Lockstep
{
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class LockstepUserRemoteMasterSystem : ISystem, ILockstepSimulationSystem,
        ISubscriber<INetIncomingMessage<DirectionInput>>
    {
        private readonly UserSimulationEntityMapper _userSimulationEntityMapper;
        private readonly ITickFactory _tickFactory;

        public LockstepUserRemoteMasterSystem(UserSimulationEntityMapper userSimulationEntityMapper, ITickFactory tickFactory)
        {
            _tickFactory = tickFactory;
            _userSimulationEntityMapper = userSimulationEntityMapper;
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
                pilotId: (ushort)_userSimulationEntityMapper.GetId(message.Peer.Id),
                which: message.Body.Which,
                value: message.Body.Value));
        }
    }
}
