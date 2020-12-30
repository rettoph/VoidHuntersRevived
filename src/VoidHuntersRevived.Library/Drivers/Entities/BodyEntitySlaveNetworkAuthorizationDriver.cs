﻿using Guppy.DependencyInjection;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Extensions.Aether;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntitySlaveNetworkAuthorizationDriver : SlaveNetworkAuthorizationDriver<BodyEntity>
    {
        #region Private Fields
        private NetClient _client;
        #endregion

        #region Lifecycle Methods
        protected override void InitializeRemote(BodyEntity driven, ServiceProvider provider)
        {
            base.InitializeRemote(driven, provider);

            provider.Service(out _client);

            this.driven.OnUpdate += this.Update;

            this.driven.MessageHandlers[MessageType.Update].OnRead += this.driven.master.ReadPosition;
        }

        protected override void ReleaseRemote(BodyEntity driven)
        {
            base.ReleaseRemote(driven);

            _client = null;

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
                var posStrength = MathHelper.Clamp(BodyEntity.SlaveLerpStrength * (Single)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                var velStrength = MathHelper.Lerp(posStrength, 1, 0.75f);

                this.driven.slave.LinearVelocity = Vector2.Lerp(this.driven.slave.LinearVelocity, this.driven.master.LinearVelocity, velStrength);
                this.driven.slave.AngularVelocity = MathHelper.Lerp(this.driven.slave.AngularVelocity, this.driven.master.AngularVelocity, velStrength);
                this.driven.slave.SetTransformIgnoreContacts(
                    position: Vector2.Lerp(this.driven.slave.Position, this.driven.master.Position, posStrength),
                    angle: MathHelper.Lerp(this.driven.slave.Rotation, this.driven.master.Rotation, posStrength));
            
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
