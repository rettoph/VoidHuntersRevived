﻿using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Loaders;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Game.Common.Loaders
{
    [AutoLoad]
    internal sealed class DefaultTeamsLoader : ITeamLoader
    {
        public void Configure(ITeamService teams)
        {
            teams.Register(Teams.TeamZero, Resources.Strings.TeamZeroName, Resources.Colors.None, Resources.Colors.None);
            teams.Register(Teams.TeamOne, Resources.Strings.TeamOneName, Resources.Colors.TeamOnePrimaryColor, Resources.Colors.TeamOneSecondaryColor);
        }
    }
}
