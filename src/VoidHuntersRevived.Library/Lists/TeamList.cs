using Guppy.DependencyInjection;
using Guppy.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.System;

namespace VoidHuntersRevived.Library.Lists
{
    public class TeamList : ServiceList<Team>
    {
        private static Guid DefaultTeamId => new Guid(12345, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
        private EntityList _entities;
        
        protected override void Initialize(ServiceProvider provider)
        {
            base.Initialize(provider);

            provider.Service(out _entities);
        }

        protected override void Release()
        {
            base.Release();
        }

        protected override T Create<T>(ServiceProvider provider, uint descriptorId, Action<T, ServiceProvider, ServiceConfiguration> setup = null, Guid? id = null)
            => _entities.Create(descriptorId, setup, id);

        public Team GetDefaultTeam()
            => this.GetOrCreateById(TeamList.DefaultTeamId);

        public Team GetNextTeam()
        {
            Team team = null;

            team = this.ElementAt((new Random()).Next(0, this.Count));

            return team;
        }
    }
}
