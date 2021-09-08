using Guppy.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Components.Entities.WorldObjects
{
    /// <summary>
    /// Detect when a <see cref="AetherBodyWorldObject.Body"/> is dirty and must
    /// be broadcasted.
    /// </summary>
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal class AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent : RemoteHostComponent<AetherBodyWorldObject>
    {
        #region Private Fields
        private Double _millisecondsSinceLastWorldInfoClean;
        #endregion

        protected override void InitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(provider, networkAuthorization);

            this.Entity.ValidateWorldInfoDirty += this.HandleValidateWorldInfoChangeDetected;
            this.Entity.OnWorldInfoDirtyChanged += this.HandleWorldInfoDirtyChanged;
        }

        protected override void ReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(networkAuthorization);

            this.Entity.ValidateWorldInfoDirty -= this.HandleValidateWorldInfoChangeDetected;
            this.Entity.OnWorldInfoDirtyChanged -= this.HandleWorldInfoDirtyChanged;
        }

        private bool HandleValidateWorldInfoChangeDetected(IWorldObject sender, GameTime args)
        {
            if ((_millisecondsSinceLastWorldInfoClean += args.ElapsedGameTime.TotalMilliseconds) < Constants.Intervals.AetherBodyWorldObjectCleanIntervalMinimum)
            {
                return false;
            }

            if(_millisecondsSinceLastWorldInfoClean > Constants.Intervals.AetherBodyWorldObjectCleanIntervalMaximum)
            {
                return true;
            }

            var linearVelocityDif = Vector2.Distance(
                this.Entity.Body.Instances[NetworkAuthorization.Master].LinearVelocity,
                this.Entity.Body.Instances[NetworkAuthorization.Slave].LinearVelocity);

            var angularVelocityDif = MathHelper.Distance(
                this.Entity.Body.Instances[NetworkAuthorization.Master].AngularVelocity,
                this.Entity.Body.Instances[NetworkAuthorization.Slave].AngularVelocity);

            if (angularVelocityDif > Constants.Thresholds.MasterBodyAngularVelocityDifferenceTheshold || linearVelocityDif > Constants.Thresholds.MasterBodyLinearVelocityDifferenceTheshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void HandleWorldInfoDirtyChanged(IWorldObject sender, bool dirty)
        {
            if(!dirty)
            { // It has just been cleaned! Reset the clock.
                this.Entity.Body.Instances[NetworkAuthorization.Slave].SetTransformIgnoreContacts(
                    this.Entity.Body.Instances[NetworkAuthorization.Master].Position,
                    this.Entity.Body.Instances[NetworkAuthorization.Master].Rotation);
                
                this.Entity.Body.Instances[NetworkAuthorization.Slave].AngularVelocity = this.Entity.Body.Instances[NetworkAuthorization.Master].AngularVelocity;
                this.Entity.Body.Instances[NetworkAuthorization.Slave].LinearVelocity = this.Entity.Body.Instances[NetworkAuthorization.Master].LinearVelocity;

                _millisecondsSinceLastWorldInfoClean = 0;
            }
        }
    }
}
