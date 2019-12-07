using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Library.Configurations
{
    public class GunConfiguration : WeaponConfiguration
    {        
        /// <summary>
        /// The handle registered to the type of Projectile entity fired
        /// by this gun.
        /// </summary>
        public String ProjectileHandle { get; private set; }
        /// <summary>
        /// The speed projectiles are fired at
        /// </summary>
        public Single FireStrength { get; private set; } = 10f;

        /// <summary>
        /// Update the weapons swivel range.
        /// </summary>
        /// <param name="projectileHandle"></param>
        public void SetProjectileHandle(String projectileHandle)
        {
            this.ProjectileHandle = projectileHandle;
        }

        /// <summary>
        /// Update the weapons fire strength.
        /// </summary>
        /// <param name="swivelRange"></param>
        public void SetFireStrength(Single fireStrength)
        {
            this.FireStrength = fireStrength;
        }
    }
}
