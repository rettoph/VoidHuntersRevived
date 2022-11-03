using Guppy.Attributes;
using Guppy.Common;
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
using VoidHuntersRevived.Library.Mappers;
using VoidHuntersRevived.Library.Messages;

namespace VoidHuntersRevived.Library.Systems
{
    [AutoLoad]
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class UserPilotSystem : EntitySystem, ISubscriber<UserPilot>
    {
        private readonly IBus _bus;
        private readonly IUserProvider _users;
        private World _world;
        private ComponentMapper<User> _userMapper;
        private readonly PilotIdMap _pilotMap;

        public UserPilotSystem(PilotIdMap userPilotMap, IBus bus, IUserProvider users) : base(Aspect.All(typeof(User), typeof(Piloting)))
        {
            _bus = bus;
            _users = users;
            _world = default!;
            _userMapper = default!;
            _pilotMap = userPilotMap;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _bus.Subscribe(this);

            _userMapper = mapperService.GetMapper<User>();
        }

        ~UserPilotSystem()
        {
            _bus.Unsubscribe(this);
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(this.subscription.IsInterested(entityId))
            {
                var user = _userMapper.Get(entityId);

                _pilotMap.Add(entityId, user.Id);
            }
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if(this.subscription.IsInterested(entityId))
            {
                _pilotMap.RemoveByEntityId(entityId);
            }
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
