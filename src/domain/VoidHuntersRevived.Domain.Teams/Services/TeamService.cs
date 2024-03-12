using Guppy.Common;
using Guppy.Resources;
using Guppy.Resources.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;
using VoidHuntersRevived.Domain.Teams.Common.Loaders;
using VoidHuntersRevived.Domain.Teams.Common.Services;

namespace VoidHuntersRevived.Domain.Teams.Services
{
    internal sealed class TeamService : ITeamService
    {
        private Dictionary<Id<ITeam>, ITeam> _teams;
        private readonly IResourceProvider _resources;

        public TeamService(IResourceProvider resources, IFiltered<ITeamLoader> loaders)
        {
            _teams = new Dictionary<Id<ITeam>, ITeam>();
            _resources = resources;

            foreach (ITeamLoader loader in loaders.Instances)
            {
                loader.Configure(this);
            }
        }

        public void Register(in Id<ITeam> id, Resource<string> name, Resource<Color> primaryColor, Resource<Color> secondaryColor)
        {
            _teams.Add(id, new Team()
            {
                Id = id,
                Name = _resources.Get(name) ?? string.Empty,
                PrimaryColor = _resources.Get(primaryColor),
                SecondaryColor = _resources.Get(secondaryColor)
            });
        }

        public ITeam GetById(Id<ITeam> id)
        {
            return _teams[id];
        }

        public IEnumerable<ITeam> GetAll()
        {
            return _teams.Values.AsEnumerable();
        }
    }
}
