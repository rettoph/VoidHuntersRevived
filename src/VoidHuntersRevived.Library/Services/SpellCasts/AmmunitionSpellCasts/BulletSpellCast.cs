using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons;
using VoidHuntersRevived.Library.Services.Spells;
using VoidHuntersRevived.Library.Services.Spells.AmmunitionSpells;

namespace VoidHuntersRevived.Library.Services.SpellCasts.AmmunitionSpellCasts
{
    public class BulletSpellCast : SpellCast
    {
        protected override Spell Cast(SpellCaster caster, float manaCost, params object[] args)
        {
            return this.spells.Create<BulletSpell>((spell, p, c) =>
            {
                spell.Caster = caster;
                spell.Weapon = args[0] as Weapon;
                spell.MaxAge = (Single)args[1];
                spell.Damage = (Single)args[2];
                spell.Position = (Vector2)args[3];
                spell.Velocity = (Vector2)args[4];
                spell.EnergyShieldDeflectionCost = (Single)args[5];
            });
        }
    }
}
