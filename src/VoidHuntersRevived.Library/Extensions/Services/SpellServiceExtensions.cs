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
            Single manaCost,
            ShieldGenerator shieldGenerator)
                => spells.TryCast(
                    VHR.SpellCasts.EnergyShieldSpellCast,
                    caster,
                    manaCost,
                    shieldGenerator);
        #endregion

        #region TryCastLaserBeamSpell Methods
        public static Spell TryCastLaserBeamSpell(
            this SpellCastService spells,
            SpellCaster caster,
            Single manaCost,
            Weapon weapon,
            Single maxAge,
            Single damagePerSecond,
            Single energyShieldDeflectionCostPerSecond)
                => spells.TryCast(
                    VHR.SpellCasts.LaserBeamSpellCast,
                    caster,
                    manaCost,
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
        #endregion
    }
}
