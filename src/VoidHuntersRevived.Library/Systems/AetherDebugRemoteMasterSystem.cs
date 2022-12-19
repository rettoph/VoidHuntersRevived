﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Enums;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Factories;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoSubscribe]
    [GuppyFilter(typeof(GameGuppy))]
    [NetAuthorizationFilter(NetAuthorization.Master)]
    internal sealed class AetherDebugRemoteMasterSystem : ISystem, ISubscriber<Tick>
    {
        private readonly ITickFactory _tickFactory;
        private AetherWorld _aether;

        public AetherDebugRemoteMasterSystem(AetherWorld aether, ITickFactory tickFactory)
        {
            _aether = aether;
            _tickFactory = tickFactory;
        }

        public void Initialize(ECSWorld world)
        {
        }

        public void Dispose()
        {
        }

        public void Process(in Tick message)
        {
            foreach (var body in _aether.BodyList)
            {
                _tickFactory.Enqueue(new BodyPosition(body.GetHashCode(), body.Position));
            }
        }
    }
}
