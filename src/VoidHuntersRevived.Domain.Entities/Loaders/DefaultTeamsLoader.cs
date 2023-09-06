using Guppy.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Domain.Entities.Loaders
{
    [AutoLoad]
    internal sealed class DefaultTeamsLoader : ITeamLoader
    {
        public void Configure(ITeamService teams)
        {
            teams.Register(TeamId.Default, Resources.Strings.DefaultTeam, Resources.Colors.None, Resources.Colors.None);
            teams.Register(TeamId.Blue, Resources.Strings.BlueTeam, Resources.Colors.Blue, Resources.Colors.LightBlue);
        }
    }
}
