using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntitySlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<BodyEntity>
    {
        #region Private Fields
        private NetClient _client;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(BodyEntity driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            provider.Service(out _client);

            this.driven.OnUpdate += this.Update;

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.driven.master.ReadPosition;
        }

        protected override void Release(BodyEntity driven)
        {
            base.Release(driven);

            this.driven.OnUpdate -= this.Update;

            this.driven.MessageHandlers[MessageType.Update].OnRead -= this.driven.master.ReadPosition;
        }
        #endregion

        #region Frame Methods
        private void Update(GameTime gameTime)
        {
            var positionDif = Vector2.Distance(this.driven.slave.Position, this.driven.master.Position);
            var rotationDif = MathHelper.Distance(this.driven.slave.Rotation, this.driven.master.Rotation);
            
            if (rotationDif > 0.0001f || positionDif > 0.001f)
            { // Only proceed with positional lerping if the slave is not already matching the master...
                var strength = BodyEntity.SlaveLerpStrength * (Single)gameTime.ElapsedGameTime.TotalSeconds;
            
                this.driven.slave.LinearVelocity = Vector2.Lerp(this.driven.slave.LinearVelocity, this.driven.master.LinearVelocity, strength);
                this.driven.slave.AngularVelocity = MathHelper.Lerp(this.driven.slave.AngularVelocity, this.driven.master.AngularVelocity, strength);
                this.driven.slave.SetTransformIgnoreContacts(
                    position: Vector2.Lerp(this.driven.slave.Position, this.driven.master.Position, strength),
                    angle: MathHelper.Lerp(this.driven.slave.Rotation, this.driven.master.Rotation, strength));
            
                // Instance snap as needed...
                if (positionDif > BodyEntity.PositionSnapThreshold)
                {
                    this.driven.slave.Position = this.driven.master.Position;
                    this.driven.slave.LinearVelocity = this.driven.master.LinearVelocity;
                }
                if (rotationDif > BodyEntity.RotationSnapThreshold)
                {
                    this.driven.slave.Rotation = this.driven.master.Rotation;
                    this.driven.slave.AngularVelocity = this.driven.master.AngularVelocity;
                }
            }
        }
        #endregion
    }
}
