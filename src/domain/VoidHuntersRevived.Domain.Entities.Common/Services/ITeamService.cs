using Guppy.Resources;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface ITeamService
    {
        ITeam GetById(Id<ITeam> id);

        IEnumerable<ITeam> GetAll();

        void Register(in Id<ITeam> id, Resource<string> name, Resource<Color> primaryColor, Resource<Color> secondaryColor);
    }
}
