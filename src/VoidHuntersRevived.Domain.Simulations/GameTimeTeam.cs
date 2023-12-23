using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities;

namespace VoidHuntersRevived.Domain.Simulations
{
    public struct GameTimeTeam
    {
        public readonly GameTime GameTime;
        public readonly ITeam Team;

        public GameTimeTeam(GameTime gameTime, ITeam team)
        {
            GameTime = gameTime;
            Team = team;
        }
    }
}
