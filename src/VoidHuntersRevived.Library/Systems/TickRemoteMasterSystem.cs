﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Identity;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Enums;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Messages;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class TickRemoteMasterSystem : ISystem, ISubscriber<Tick>
    {
        private readonly NetScope _netScope;

        public TickRemoteMasterSystem(NetScope netScope)
        {
            _netScope = netScope;
        }

        public void Initialize(ECSWorld world)
        {
            // throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public void Process(in Tick tick)
        {
            _netScope.Create<Tick>(in tick)
                .AddRecipients(_netScope.Users.Peers)
                .Enqueue();
        }
    }
}
