using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using Guppy.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Guppy.Lists;
using VoidHuntersRevived.Library.Contexts;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
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

        #region Weapon Implementation
        /// <inheritdoc />
        protected override Ammunition Fire(ServiceProvider provider, EntityList entities, Action<Ammunition, ServiceProvider, ServiceConfiguration> setup)
        {
            return entities.Create<Bullet>((b, p, d) =>
            {
                setup(b, p, d);

                b.Damage = this.Context.BulletDamage;
                b.Position = this.Position;
                b.Velocity = this.Root.LinearVelocity + Vector2.Transform(Vector2.UnitX * this.Context.BulletSpeed, Matrix.CreateRotationZ(MathHelper.WrapAngle(this.Rotation + MathHelper.Pi)));
                b.ShieldDeflectionEnergyCost = this.Context.ShieldDeflectionEnergyCost;
            });
        }
        #endregion
    }
}
