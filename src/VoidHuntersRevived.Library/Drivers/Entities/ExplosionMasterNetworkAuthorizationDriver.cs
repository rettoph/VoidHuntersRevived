using Guppy.DependencyInjection;
using Guppy.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class ExplosionMasterNetworkAuthorizationDriver : MasterNetworkAuthorizationDriver<Explosion>
    {
        #region Private Fields
        private Synchronizer _synchronizer;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Explosion driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _synchronizer);

            this.driven.OnUpdate += this.Update;
            this.driven.OnImpulseApplied += this.HandleImpulseApplied;
        }

        protected override void Release(Explosion driven)
        {
            base.Release(driven);

            this.driven.OnUpdate -= this.Update;
            this.driven.OnImpulseApplied -= this.HandleImpulseApplied;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            // Auto release the explosion when its fully exploded
            if (this.driven.Age >= this.driven.MaxAge)
                this.driven.TryRelease();
        }
        #endregion

        #region Event Handlers
        private void HandleImpulseApplied(Explosion explosion, BodyEntity target, float force, Vector2 forceVector, Single elapsedSeconds)
        {
            if(target is ShipPart shipPart)
            {
                if (shipPart.Chain.Ship == default && shipPart.Health <= 0)
                    _synchronizer.Enqueue(gt => shipPart.TryRelease());
                else
                    shipPart.Health = Math.Max(0, shipPart.Health - (explosion.Damage * elapsedSeconds));
            }
        }
        #endregion
    }
}
