using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Extensions.Services
{
    public static class SpellServiceExtensions
    {
        public static Spell TryCastLaunchDrone(
            this SpellCastService spells, 
            SpellCaster caster,
            Single manaCost,
            Vector2 position, 
            Single rotation, 
            Single maxAge, 
            String type, 
            Guid team)
                => spells.TryCast(
                    VHR.SpellCasts.LaunchDronesSpellCast,
                    caster,
                    manaCost,
                    position,
                    rotation,
                    maxAge,
                    type,
                    team);
    }
}
