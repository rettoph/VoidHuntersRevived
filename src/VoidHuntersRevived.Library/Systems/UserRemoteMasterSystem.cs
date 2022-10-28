using Guppy.Attributes;
using Guppy.Common;
using Guppy.ECS.Attributes;
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
using VoidHuntersRevived.Library.Messages.Inputs;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoLoad]
    [GuppySystem(typeof(GameGuppy))]
    [NetAuthorizationSystem(NetAuthorization.Master)]
    internal sealed class UserRemoteMasterSystem : ISystem,
        ISubscriber<INetIncomingMessage<DirectionInput>>
    {
        private IBus _bus;

        public UserRemoteMasterSystem(IBus bus)
        {
            _bus = bus;
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
            throw new NotImplementedException();
        }
    }
}
