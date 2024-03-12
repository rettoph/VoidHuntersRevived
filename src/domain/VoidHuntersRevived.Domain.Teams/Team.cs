using Guppy.Resources;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams;

namespace VoidHuntersRevived.Domain.Teams
{
    internal class Team : ITeam
    {
        public required Id<ITeam> Id { get; init; }

        public required string Name { get; init; }

        public required ResourceValue<Color> PrimaryColor { get; init; }

        public required ResourceValue<Color> SecondaryColor { get; init; }
    }
}
