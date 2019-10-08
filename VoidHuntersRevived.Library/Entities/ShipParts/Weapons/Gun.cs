using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities.Ammo;

namespace GalacticFighters.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Weapon primarily used to fire multiple projectiles
    /// </summary>
    public class Gun : Weapon
    {
        #region Private Attributes
        private List<Projectile> _bullets;
        #endregion

        #region Constructors
        public Gun(World world) : base(world)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            _bullets = new List<Projectile>();
        }

        public override void Dispose()
        {
            base.Dispose();

            _bullets.Clear();
        }
        #endregion

        /// <summary>
        /// Create a new projectile
        /// </summary>
        protected override void Fire()
        {
            this.entities.Create<Projectile>("bullet", p =>
            {
                p.Gun = this;
            });
        }
    }
}
