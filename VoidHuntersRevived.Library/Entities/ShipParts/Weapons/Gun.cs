using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Specific kind of weapon designed to fire
    /// Projectile instances.
    /// </summary>
    public class Gun : Weapon
    {
        /// <summary>
        /// The handle registered to the type of Projectile entity fired
        /// by this gun.
        /// </summary>
        public String ProjectileHandle { get; set; }
        /// <summary>
        /// The speed projectiles are fired at
        /// </summary>
        public Single FireStrength { get; set; } = 10f;

        #region Weapon Implementation
        protected override void Fire()
        {
            // Auto create a projectile based on the current gun's configuration
            this.entities.Create<Projectile>(this.ProjectileHandle, p =>
            {
                var rotation = this.Joint.JointAngle + this.MaleConnectionNode.Target.WorldRotation + this.MaleConnectionNode.LocalRotation;
                p.Weapon = this;
                p.Position = this.Joint.WorldAnchorB;
                p.Rotation = rotation;
                p.Velocity = this.LinearVelocity + (Vector2.UnitX * this.FireStrength).Rotate(rotation);
            });
        }
        #endregion
    }
}
