using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts;
using VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons;
using VoidHuntersRevived.Library.Services;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Extensions.Services
{
    public static class SpellServiceExtensions
    {
        #region TryCastEnergyShieldSpell Methods
        public static Spell TryCastEnergyShieldSpell(
            this SpellCastService spells,
            SpellCaster caster,
            Boolean force,
            ShieldGenerator shieldGenerator)
                => spells.TryCast(
                    VHR.SpellCasts.EnergyShieldSpellCast,
                    caster,
                    0, // This is a 0 mana cost spell,
                    force,
                    shieldGenerator);
        #endregion

        #region TryCastLaserBeamSpell Methods
        public static Spell TryCastLaserBeamSpell(
            this SpellCastService spells,
            SpellCaster caster,
            Single manaCost,
            Boolean force,
            Weapon weapon,
            Single maxAge,
            Single damagePerSecond,
            Single energyShieldDeflectionCostPerSecond)
                => spells.TryCast(
                    VHR.SpellCasts.LaserBeamSpellCast,
                    caster,
                    manaCost,
                    force,
                    weapon,
                    maxAge,
                    damagePerSecond,
                    energyShieldDeflectionCostPerSecond);
        #endregion

        #region TryCastBulletSpell Methods
        public static Spell TryCastBulletSpell(
            this SpellCastService spells,
            SpellCaster caster,
            Single manaCost,
            Boolean force,
            Weapon weapon,
            Single maxAge,
            Single damage,
            Vector2 position,
            Vector2 velocity,
            Single energyShieldDeflectionCost)
                => spells.TryCast(
                    VHR.SpellCasts.BulletSpellCast,
                    caster,
                    manaCost,
                    force,
                    weapon,
                    maxAge,
                    damage,
                    position,
                    velocity,
                    energyShieldDeflectionCost);
        #endregion

        #region TryCastLaunchDrone Methods
        public static Spell TryCastLaunchDroneSpell(
            this SpellCastService spells, 
            SpellCaster caster,
            Single manaCost,
            Boolean force,
            Vector2 position, 
            Single rotation, 
            Single maxAge, 
            String type, 
            Team team)
                => spells.TryCast(
                    VHR.SpellCasts.LaunchDronesSpellCast,
                    caster,
                    manaCost,
                    force,
                    position,
                    rotation,
                    maxAge,
                    type,
                    team);
        #endregion
    }
}
