﻿using Guppy.DependencyInjection;
using Guppy.Enums;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Extensions.Lidgren;
using Guppy.Network.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Components.Entities.Players
{
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class AetherBodySlaveLerpComponent : NetworkComponent<AetherBody>
    {
        #region Lifecycle Methods
        protected override void PreInitializeRemote(GuppyServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.PreInitializeRemote(provider, networkAuthorization);

            this.Entity.OnUpdate += this.Update;
        }

        protected override void PostReleaseRemote(NetworkAuthorization networkAuthorization)
        {
            base.PostReleaseRemote(networkAuthorization);

            this.Entity.OnUpdate -= this.Update;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            var positionDif = Vector2.Distance(
                this.Entity.Instances[NetworkAuthorization.Slave].Position, 
                this.Entity.Instances[NetworkAuthorization.Master].Position);

            var rotationDif = MathHelper.Distance(
                this.Entity.Instances[NetworkAuthorization.Slave].Rotation, 
                this.Entity.Instances[NetworkAuthorization.Master].Rotation);

            if (rotationDif > Constants.Thresholds.SlaveBodyRotationSnapThreshold || positionDif > Constants.Thresholds.SlaveBodyPositionSnapThreshold)
            { // Instant snap if the difference is to great
                this.Entity.Instances[NetworkAuthorization.Slave].SetTransformIgnoreContacts(
                    this.Entity.Instances[NetworkAuthorization.Master].Position,
                    this.Entity.Instances[NetworkAuthorization.Master].Rotation);

                this.Entity.Instances[NetworkAuthorization.Slave].AngularVelocity = this.Entity.Instances[NetworkAuthorization.Master].AngularVelocity;
                this.Entity.Instances[NetworkAuthorization.Slave].LinearVelocity = this.Entity.Instances[NetworkAuthorization.Master].LinearVelocity;
            }
            else if (rotationDif > Constants.Thresholds.SlaveBodyRotationDifferenceTheshold || positionDif > Constants.Thresholds.SlaveBodyPositionDifferenceTheshold)
            { // Only proceed with positional lerping if the slave is not already matching the master...
                var posStrength = MathHelper.Clamp(Constants.LerpStrengths.SlaveBodyLerpStrength * (Single)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                var velStrength = MathHelper.Lerp(posStrength, 1, 0.75f);

                this.Entity.Instances[NetworkAuthorization.Slave].LinearVelocity = Vector2.Lerp(
                    this.Entity.Instances[NetworkAuthorization.Slave].LinearVelocity, 
                    this.Entity.Instances[NetworkAuthorization.Master].LinearVelocity, 
                    velStrength);

                this.Entity.Instances[NetworkAuthorization.Slave].AngularVelocity = MathHelper.Lerp(
                    this.Entity.Instances[NetworkAuthorization.Slave].AngularVelocity, 
                    this.Entity.Instances[NetworkAuthorization.Master].AngularVelocity, 
                    velStrength);

                this.Entity.Instances[NetworkAuthorization.Slave].SetTransformIgnoreContacts(
                    position: Vector2.Lerp(
                        this.Entity.Instances[NetworkAuthorization.Slave].Position, 
                        this.Entity.Instances[NetworkAuthorization.Master].Position, 
                        posStrength),
                    angle: MathHelper.Lerp(
                        this.Entity.Instances[NetworkAuthorization.Slave].Rotation, 
                        this.Entity.Instances[NetworkAuthorization.Master].Rotation, 
                        posStrength));
            }
        }
        #endregion
    }
}
