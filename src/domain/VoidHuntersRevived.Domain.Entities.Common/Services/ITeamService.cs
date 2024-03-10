using Guppy.Resources;
using Microsoft.Xna.Framework;

namespace VoidHuntersRevived.Domain.Entities.Common.Services
{
    public interface ITeamService : IEntityResourceService<ITeam>
    {
        void Register(in Id<ITeam> id, Resource<string> name, Resource<Color> primaryColor, Resource<Color> secondaryColor);
    }
}
