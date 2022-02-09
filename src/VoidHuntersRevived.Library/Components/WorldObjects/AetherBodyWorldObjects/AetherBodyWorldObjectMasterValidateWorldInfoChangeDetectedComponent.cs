using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Interfaces;
using VoidHuntersRevived.Library.Messages.Network;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    /// <summary>
    /// Detect when a <see cref="AetherBodyWorldObject.Body"/> is dirty and must
    /// be broadcasted.
    /// </summary>
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Master)]
    internal class AetherBodyWorldObjectMasterValidateWorldInfoChangeDetectedComponent : Component<AetherBodyWorldObject>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.OnUpdate += this.CheckDirty;
            this.Entity.Messages.GetPinger<WorldObjectPositionPing>().OnDirtyChanged += this.HandleWorldInfoDirtyChanged;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.OnUpdate -= this.CheckDirty;
            this.Entity.Messages.GetPinger<WorldObjectPositionPing>().OnDirtyChanged -= this.HandleWorldInfoDirtyChanged;
        }
        #endregion

        private void CheckDirty(GameTime gameTime)
        {
            this.Entity.Messages.GetPinger<WorldObjectPositionPing>().Enabled = this.Entity.Body.LocalInstance.Awake;

            var linearVelocityDif = Vector2.Distance(
                this.Entity.Body.Instances[NetworkAuthorization.Master].LinearVelocity,
                this.Entity.Body.Instances[NetworkAuthorization.Slave].LinearVelocity);

            if (linearVelocityDif > Thresholds.MasterBodyLinearVelocityDifferenceTheshold)
            {
                this.Entity.Messages.GetPinger<WorldObjectPositionPing>().Dirty = true;
                return;
            }

            var angularVelocityDif = MathHelper.Distance(
                this.Entity.Body.Instances[NetworkAuthorization.Master].AngularVelocity,
                this.Entity.Body.Instances[NetworkAuthorization.Slave].AngularVelocity);

            if (angularVelocityDif > Thresholds.MasterBodyAngularVelocityDifferenceTheshold)
            {
                this.Entity.Messages.GetPinger<WorldObjectPositionPing>().Dirty = true;
                return;
            }
        }

        private void HandleWorldInfoDirtyChanged(NetworkEntityMessagePinger sender, bool dirty)
        {
            if(!dirty)
            { // It has just been cleaned! Reset the clock.
                this.Entity.Body.Instances[NetworkAuthorization.Slave].SetTransformIgnoreContacts(
                    this.Entity.Body.Instances[NetworkAuthorization.Master].Position,
                    this.Entity.Body.Instances[NetworkAuthorization.Master].Rotation);
                
                this.Entity.Body.Instances[NetworkAuthorization.Slave].AngularVelocity = this.Entity.Body.Instances[NetworkAuthorization.Master].AngularVelocity;
                this.Entity.Body.Instances[NetworkAuthorization.Slave].LinearVelocity = this.Entity.Body.Instances[NetworkAuthorization.Master].LinearVelocity;
            }
        }
    }
}
