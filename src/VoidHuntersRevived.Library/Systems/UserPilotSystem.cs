using Guppy.Attributes;
using Guppy.Common;
using Guppy.ECS.Attributes;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using Guppy.Network.Peers;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Library.Attributes;
using VoidHuntersRevived.Library.Components;
using VoidHuntersRevived.Library.Helpers;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoLoad]
    [GuppySystem(typeof(GameGuppy))]
    internal sealed class UserPilotSystem : EntitySystem, ISubscriber<UserPilot>
    {
        private IBus _bus;
        private IUserProvider _users;
        private World _world;


        public UserPilotSystem(IBus bus, IUserProvider users) : base(Aspect.All(typeof(User), typeof(Piloting)))
        {
            _bus = bus;
            _users = users;
            _world = default!;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bus.Subscribe(this);
        }

        ~UserPilotSystem()
        {
            _bus.Unsubscribe(this);
        }

        public void Process(in UserPilot message)
        {
            var user = _users.UpdateOrCreate(message.User.Id, message.User.Claims);

            switch (message.User.Action)
            {
                case UserAction.Actions.UserJoined:
                    var pilot = EntityHelper.Pilots.CreateUserPilot(_world, user);
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
