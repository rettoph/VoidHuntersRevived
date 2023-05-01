using Guppy.Attributes;
using Guppy.Network.Identity;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Components;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserPilotSystem : EntitySystem
    {
        private static readonly AspectBuilder UserPilotAspect = Aspect.All(new[]
        {
            typeof(User),
            typeof(Piloting),
            typeof(Parallelable)
        });

        private readonly IUserPilotMappingService _userPilots;
        private ComponentMapper<User> _users;
        private ComponentMapper<Parallelable> _parallelables;

        public UserPilotSystem(IUserPilotMappingService userPilots) : base(UserPilotAspect)
        {
            _userPilots = userPilots;
            _users = null!;
            _parallelables = null!;
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _users = mapperService.GetMapper<User>();
            _parallelables = mapperService.GetMapper<Parallelable>();
        }

        protected override void OnEntityAdded(int entityId)
        {
            base.OnEntityAdded(entityId);

            if(!this.subscription.IsInterested(entityId))
            {
                return;
            }

            Parallelable parallelable = _parallelables.Get(entityId);
            User user = _users.Get(entityId);

            _userPilots.Add(user.Id, parallelable.Key);
        }

        protected override void OnEntityRemoved(int entityId)
        {
            base.OnEntityRemoved(entityId);

            if (!this.subscription.IsInterested(entityId))
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
