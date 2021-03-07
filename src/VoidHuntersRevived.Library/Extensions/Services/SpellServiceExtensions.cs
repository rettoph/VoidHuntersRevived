using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Spells;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Extensions.Services
{
    public static class SpellServiceExtensions
    {
        public static LaunchDroneSpell CastLaunchDrone(
            this SpellService spells, 
            Vector2 position, 
            Single rotation, 
            Single maxAge, 
            String type, 
            Guid team)
                => spells.Cast<LaunchDroneSpell>((launchDronSpell, p, c) =>
                {
                    launchDronSpell.Position = position;
                    launchDronSpell.Rotation = rotation;
                    launchDronSpell.MaxAge = maxAge;
                    launchDronSpell.Type = type;
                    launchDronSpell.Team = team;
                });
    }
}
