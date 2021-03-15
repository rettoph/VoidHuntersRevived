using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Guppy.Lists;
using VoidHuntersRevived.Library.Contexts;
using VoidHuntersRevived.Library.Extensions.Services;
using VoidHuntersRevived.Library.Services.Spells;

namespace VoidHuntersRevived.Library.Entities.ShipParts.SpellParts.Weapons
{
    /// <summary>
    /// Simple weapon implementation that will fire
    /// projectiles
    /// </summary>
    public class Gun : Weapon
    {
        #region Public Properties
        public new GunContext Context { get; private set; }
        #endregion

        #region Helper Methods
        public override void SetContext(ShipPartContext context)
        {
            base.SetContext(context);

            this.Context = context as GunContext;
        }
        #endregion

        #region SpellCaster Implementation
        /// <inheritdoc />
        protected override Spell Cast(GameTime gameTime)
            => this.spells.TryCastBulletSpell(
                caster: this.Chain.Ship,
                manaCost: this.Context.SpellManaCost,
                weapon: this,
                maxAge: this.Context.MaximumAmmunitionAge,
                damage: this.Context.BulletDamage,
                position: this.Position,
                velocity: this.Root.LinearVelocity + Vector2.Transform(Vector2.UnitX * this.Context.BulletSpeed, Matrix.CreateRotationZ(MathHelper.WrapAngle(this.Rotation + MathHelper.Pi))),
                energyShieldDeflectionCost: this.Context.EnergyShieldDeflectionCost);
        #endregion
    }
}
