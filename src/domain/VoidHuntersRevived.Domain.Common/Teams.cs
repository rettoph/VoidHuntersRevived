using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Common
{
    public static class Teams
    {
        public static readonly Id<ITeam> TeamZero = Id<ITeam>.FromString(nameof(TeamZero));
        public static readonly Id<ITeam> TeamOne = Id<ITeam>.FromString(nameof(TeamOne));
    }
}
