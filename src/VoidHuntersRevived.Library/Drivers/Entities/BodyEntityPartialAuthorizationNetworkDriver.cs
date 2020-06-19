using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntityPartialAuthorizationNetworkDriver : BaseAuthorizationDriver<BodyEntity>
    {
        #region Lifecycle Methods
        protected override void ConfigurePartial(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnUpdate += this.Update;

            this.driven.Actions.Set("update:position", this.ReadPosition);
        }

        protected override void DisposePartial()
        {
            base.DisposePartial();

            this.driven.OnUpdate -= this.Update;

            this.driven.Actions.Remove("update:position");
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            if (this.driven.Authorization == GameAuthorization.Partial)
            {
                var positionDif = Vector2.Distance(this.driven.slave.Position, this.driven.master.Position);
                var rotationDif = MathHelper.Distance(this.driven.slave.Rotation, this.driven.master.Rotation);

                if (rotationDif > 0.0001f || positionDif > 0.001f)
                { // Only proceed with positional lerping if the slave is not already matching the master...
                    var strength = BodyEntity.SlaveLerpStrength * (Single)gameTime.ElapsedGameTime.TotalMilliseconds;

                    this.driven.slave.LinearVelocity = Vector2.Lerp(this.driven.slave.LinearVelocity, this.driven.master.LinearVelocity, strength);
                    this.driven.slave.AngularVelocity = MathHelper.Lerp(this.driven.slave.AngularVelocity, this.driven.master.AngularVelocity, strength);
                    this.driven.slave.SetTransformIgnoreContacts(
                        position: Vector2.Lerp(this.driven.slave.Position, this.driven.master.Position, strength),
                        angle: MathHelper.Lerp(this.driven.slave.Rotation, this.driven.master.Rotation, strength));

                    // Instance snap as needed...
                    if (positionDif > BodyEntity.PositionSnapThreshold)
                        this.driven.slave.Position = this.driven.master.Position;
                    if (rotationDif > BodyEntity.RotationSnapThreshold)
                        this.driven.slave.Rotation = this.driven.master.Rotation;
                }
            }
            else
            {
                // Instant snap the slave...
                this.driven.slave.Position = this.driven.master.Position;
                this.driven.slave.Rotation = this.driven.master.Rotation;
            }
        }
        #endregion

        #region Network Methods
        private void ReadPosition(NetIncomingMessage im)
        {
            if (this.driven.Authorization == GameAuthorization.Partial)
            {
                this.driven.Position = im.ReadVector2();
                this.driven.Rotation = im.ReadSingle();

                this.driven.master.LinearVelocity = im.ReadVector2();
                this.driven.master.AngularVelocity = im.ReadSingle();
            }
            else
            { // Skip the bits that would have been read...
                im.Position += 192;
            }
        }
        #endregion
    }
}
