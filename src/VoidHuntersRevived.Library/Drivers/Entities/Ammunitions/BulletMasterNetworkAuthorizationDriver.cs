using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.Ammunitions
{
    internal sealed class BulletMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Bullet>
    {
        #region Lifecycle Methods
        protected override void Initialize(Bullet driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            driven.OnCollision += this.HandleBulletCollision;
        }

        protected override void Release(Bullet driven)
        {
            base.Release(driven);

            driven.OnCollision -= this.HandleBulletCollision;
        }
        #endregion

        #region Event Handlers
        private void HandleBulletCollision(Ammunition bullet, ShipPart shipPart)
        {
            // Apply the current damage output of the bullet the the hit ship part.
            shipPart.TryDamage(this.driven.Damage);
        }
        #endregion
    }
}
