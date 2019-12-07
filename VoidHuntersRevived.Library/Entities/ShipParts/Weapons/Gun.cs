using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Configurations;
using VoidHuntersRevived.Library.Entities.Ammunitions;

namespace VoidHuntersRevived.Library.Entities.ShipParts.Weapons
{
    /// <summary>
    /// Specific kind of weapon designed to fire
    /// Projectile instances.
    /// </summary>
    public class Gun : Weapon
    {

        #region Weapon Implementation
        protected override void Fire()
        {
            // Auto create a projectile based on the current gun's configuration
            this.entities.Create<Projectile>(this.Configuration.GetData<GunConfiguration>().ProjectileHandle, p =>
            {
                var rotation = this.Joint.JointAngle + this.MaleConnectionNode.Target.WorldRotation + this.MaleConnectionNode.LocalRotation;
                p.Weapon = this;
                p.Position = this.Joint.WorldAnchorB;
                p.Rotation = rotation;
                p.Velocity = this.Root.LinearVelocity + Vector2.Transform(Vector2.UnitX * this.Configuration.GetData<GunConfiguration>().FireStrength, Matrix.CreateRotationZ(rotation));
            });
        }
        #endregion
    }
}
