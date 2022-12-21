﻿using Guppy.Attributes;
using Guppy.Common;
using Guppy.Network.Enums;
using Guppy.Network.Identity;
using Guppy.Network.Identity.Providers;
using Guppy.Network.Identity.Services;
using Guppy.Network.Messages;
using Guppy.Network.Peers;
using Microsoft.Xna.Framework;
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
    [GuppyFilter(typeof(GameGuppy))]
    internal sealed class UserPilotSystem : EntitySystem, ISubscriber<UserPilot>
    {
        private readonly IUserProvider _userProvider;
        private World _world;
        private ComponentMapper<User> _users;
        private ComponentMapper<Piloting> _pilotings;
        private readonly PilotIdMap _pilotMap;
        private readonly AetherWorld _aether;

        public UserPilotSystem(
            PilotIdMap userPilotMap,
            AetherWorld aether,
            IUserProvider userProvider) : base(Aspect.All(typeof(User), typeof(Piloting)))
        {
            _userProvider = userProvider;
            _world = default!;
            _users = default!;
            _pilotings = default!;
            _pilotMap = userPilotMap;
            _aether = aether;
        }

        public override void Initialize(World world)
        {
            base.Initialize(world);

            _world = world;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _users = mapperService.GetMapper<User>();
            _pilotings = mapperService.GetMapper<Piloting>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(this.subscription.IsInterested(entityId))
            {
                var user = _users.Get(entityId);

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
            var user = _userProvider.UpdateOrCreate(message.User.Id, message.User.Claims);

            switch (message.User.Action)
            {
                case UserAction.Actions.UserJoined:
                    var pilot = EntityHelper.Pilots.CreateUserPilot(_world, user);
                    var ship = EntityHelper.Rootables.CreateShip(_world, _aether.CreateRectangle(1, 1, 1, Vector2.Zero, 0, AetherBodyType.Dynamic));

                    _pilotings.Get(pilot).PilotableId = ship.Id;
                    break;
                default:
                    throw new Exception();
            }
        }
    }
}
