using Guppy.Resources;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Services
{
    public interface ITeamService
    {
        void Register(in TeamId id, Resource<string> name, Resource<Color> primaryColor, Resource<Color> secondaryColor);

        IEnumerable<ITeam> All();
    }
}
