using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
