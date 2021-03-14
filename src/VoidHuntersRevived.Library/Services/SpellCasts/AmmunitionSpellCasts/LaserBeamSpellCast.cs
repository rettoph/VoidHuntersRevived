using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons;
using VoidHuntersRevived.Library.Services.Spells;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

namespace VoidHuntersRevived.Library.Services.SpellCasts.AmmunitionSpellCasts
{
    public class LaserBeamSpellCast : SpellCast
    {
        /// <inheritdoc />
        protected override Spell Cast(SpellCaster caster, Single manaCost, params object[] args)
        {
            return this.spells.Create<LaserBeamSpell>((spell, p, c) =>
            {
                spell.Caster = caster;
                spell.Weapon = args[0] as Weapon;
                spell.MaxAge = (Single)args[1];
                spell.DamagePerSecond = (Single)args[2];
                spell.EnergyShieldDeflectionCostPerSecond = (Single)args[3];
            });
        }
    }
}
