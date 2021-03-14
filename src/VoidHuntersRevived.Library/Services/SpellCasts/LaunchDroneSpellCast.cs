using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Services.SpellCasts
{
    public class LaunchDroneSpellCast : SpellCast
    {
        /// <inheritdoc />
        protected override Spell Cast(SpellCaster caster, Single manaCost, params object[] args)
        {
            return this.spells.Create<LaunchDroneSpell>((spell, p, c) =>
            {
                spell.Caster = caster;
                spell.Position = (Vector2)args[0];
                spell.Rotation = (Single)args[1];
                spell.MaxAge = (Single)args[2];
                spell.Type = (String)args[3];
                spell.Team = (Guid)args[4];
            });
        }
    }
}
