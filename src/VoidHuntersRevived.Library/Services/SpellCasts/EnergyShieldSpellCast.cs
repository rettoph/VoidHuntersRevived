using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Services.SpellCasts
{
    public class EnergyShieldSpellCast : SpellCast
    {
        protected override Spell Cast(SpellCaster caster, Single manaCost, params Object[] args)
        {
            return this.spells.Create<EnergyShieldSpell>((spell, p, c) =>
            {
                spell.Caster = caster;
                spell.ShieldGenerator = args[0] as ShieldGenerator;
            });
        }
    }
}
