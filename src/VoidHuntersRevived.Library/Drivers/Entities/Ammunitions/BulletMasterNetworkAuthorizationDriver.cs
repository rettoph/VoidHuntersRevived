using Guppy.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Ammunitions;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts.Special;
using VoidHuntersRevived.Library.Enums;

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
        private void HandleBulletCollision(Ammunition bullet, Ammunition.CollisionDataResult collision)
        {
            if ((collision.Result & ShipPartAmmunitionCollisionResult.Damage) != 0)
                collision.Data.Target.TryDamage(this.driven.Damage);
        }
        #endregion
    }
}
