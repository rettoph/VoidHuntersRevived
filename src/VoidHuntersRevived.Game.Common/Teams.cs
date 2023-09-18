using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Game.Common
{
    public static class Teams
    {
        public static readonly Id<ITeam> TeamZero = Id<ITeam>.FromString(nameof(TeamZero));
        public static readonly Id<ITeam> TeamOne = Id<ITeam>.FromString(nameof(TeamOne));
    }
}
