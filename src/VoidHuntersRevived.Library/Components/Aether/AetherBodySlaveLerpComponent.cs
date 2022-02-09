using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Aether;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Scenes;
using VoidHuntersRevived.Library.Extensions.Aether;
using VoidHuntersRevived.Library.Globals.Constants;
using Guppy.EntityComponent;

namespace VoidHuntersRevived.Library.Components.Aether
{
    [HostTypeRequired(HostType.Remote)]
    [NetworkAuthorizationRequired(NetworkAuthorization.Slave)]
    internal sealed class AetherBodySlaveLerpComponent : Component<AetherBody>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Entity.OnUpdate += this.Update;
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

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

            if (rotationDif > Thresholds.SlaveBodyRotationSnapThreshold || positionDif > Thresholds.SlaveBodyPositionSnapThreshold)
            { // Instant snap if the difference is to great
                this.Entity.Instances[NetworkAuthorization.Slave].SetTransformIgnoreContacts(
                    this.Entity.Instances[NetworkAuthorization.Master].Position,
                    this.Entity.Instances[NetworkAuthorization.Master].Rotation);

                this.Entity.Instances[NetworkAuthorization.Slave].AngularVelocity = this.Entity.Instances[NetworkAuthorization.Master].AngularVelocity;
                this.Entity.Instances[NetworkAuthorization.Slave].LinearVelocity = this.Entity.Instances[NetworkAuthorization.Master].LinearVelocity;
            }
            else if (rotationDif > Thresholds.SlaveBodyRotationDifferenceTheshold || positionDif > Thresholds.SlaveBodyPositionDifferenceTheshold)
            { // Only proceed with positional lerping if the slave is not already matching the master...
                var posStrength = MathHelper.Clamp(LerpStrengths.SlaveBodyLerpStrength * (Single)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
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
