using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;

namespace VoidHuntersRevived.Library.Drivers.Entities.Ammunitions
{
    internal sealed class LaserBeamMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<LaserBeam>
    {
        #region Lifecycle Methods
        protected override void Initialize(LaserBeam driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            driven.OnCollision += this.HandleBulletCollision;
        }

        protected override void Release(LaserBeam driven)
        {
            base.Release(driven);

            driven.OnCollision -= this.HandleBulletCollision;
        }
        #endregion

        #region Event Handlers
        private void HandleBulletCollision(Ammunition bullet, Ammunition.CollisionData data)
        {
            // Check to see if the ammo hit a shield...
            if (data.Fixture.Tag is ShieldGenerator shield && data.Fixture.IsSensor)
                return;

            // Apply the current damage output of the bullet the the hit ship part.
            data.Target.TryDamage(this.driven.DamagePerSecond * (Single)data.GameTime.ElapsedGameTime.TotalSeconds);
        }
        #endregion
    }
}
