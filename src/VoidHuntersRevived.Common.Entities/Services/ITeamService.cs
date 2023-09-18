using Guppy.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface ITeamService : IEntityResourceService<ITeam>
    {
        void Register(in Id<ITeam> id, Resource<string> name, Resource<Color> primaryColor, Resource<Color> secondaryColor);
    }
}
