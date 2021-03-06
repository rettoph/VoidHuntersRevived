﻿using Guppy.DependencyInjection;
using Guppy.Lists;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Extensions.Services;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons
{
    public class Laser : Weapon
    {
        #region Public Properties
        public new LaserContext Context { get; private set; }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as LaserContext;
        }
        #endregion

        #region SpellCaster Implementation
        /// <inheritdoc />
        protected override Spell Cast(GameTime gameTime, Boolean force = false)
            => this.spells.TryCastLaserBeamSpell(
                    caster: this.Chain.Ship,
                    manaCost: this.Context.SpellManaCost,
                    force: force,
                    weapon: this,
                    maxAge: this.Context.MaximumAmmunitionAge,
                    damagePerSecond: this.Context.DamagePerSecond,
                    energyShieldDeflectionCostPerSecond: this.Context.ShieldDeflectionEnergyCostPerSecond);
        #endregion
    }
}
