using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Entities
{
    internal class Team : ITeam
    {
        public Id<ITeam> Id { get; init; }

        public string Name { get; init; }

        public Color PrimaryColor { get; init; }

        public Color SecondaryColor { get; init; }
    }
}
