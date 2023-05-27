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
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Components;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Common.Simulations.Systems;

namespace VoidHuntersRevived.Domain.Simulations.Systems
{
    [GuppyFilter<IGameGuppy>()]
    internal sealed class UserShipSystem : ParallelEntitySystem
    {
        private static readonly AspectBuilder UserPilotAspect = Aspect.All(new[]
        {
            typeof(User),
            typeof(Helm)
        });

        private readonly IUserShipMappingService _userShips;
        private IParallelComponentMapper<User> _users;

        public UserShipSystem(IUserShipMappingService userShips) : base(UserPilotAspect)
        {
            _userShips = userShips;
            _users = null!;
        }

        public override void Initialize(IParallelComponentMapperService components, IParallelEntityService entities)
        {
            base.Initialize(components, entities);

            _users = components.GetMapper<User>();
        }

        protected override void OnEntityAdded(ParallelKey entityKey, ISimulation simulation)
        {
            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            User user = _users.Get(entityKey, simulation);

            _userShips.Add(user.Id, entityKey);
        }

        protected override void OnEntityRemoved(ParallelKey entityKey, ISimulation simulation)
        {
            if (!this.Entities[simulation.Type].IsInterested(entityKey))
            {
                return;
            }

            throw new NotImplementedException();
        }
    }
}
