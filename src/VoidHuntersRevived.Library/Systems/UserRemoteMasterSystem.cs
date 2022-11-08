﻿using Guppy.Attributes;
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
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Systems
{
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationSystem(NetAuthorization.Master)]
    internal sealed class UserRemoteMasterSystem : ISystem,
        ISubscriber<INetIncomingMessage<DirectionInput>>
    {
        private readonly PilotIdMap _userPilotMap;
        private readonly IBus _bus;
        private readonly ITickFactory _tickFactory;

        public UserRemoteMasterSystem(PilotIdMap userPilotMap, IBus bus, ITickFactory tickFactory)
        {
            _bus = bus;
            _tickFactory = tickFactory;
            _userPilotMap = userPilotMap;
        }

        public void Initialize(World world)
        {
            _bus.Subscribe(this);
        }

        public void Dispose()
        {
            _bus.Unsubscribe(this);
        }

        public void Process(in INetIncomingMessage<DirectionInput> message)
        {
            if(message.Peer is null)
            {
                return;
            }

            _tickFactory.Enqueue(new PilotDirectionInput(
                pilotId: _userPilotMap.GetNetIdFromUserId(message.Peer.Id),
                which: message.Body.Which,
                value: message.Body.Value));
        }
    }
}
