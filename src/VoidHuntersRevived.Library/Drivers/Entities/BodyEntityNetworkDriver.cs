using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    internal sealed class BodyEntityNetworkDriver : NetworkEntityNetworkDriver<BodyEntity>
    {
        #region Private Fields
        private ActionTimer _timer;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            _timer = new ActionTimer(150);

            this.AddAction("update:position", this.ReadPosition, (GameAuthorization.Full, this.SkipPosition));
        }

        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            _timer = new ActionTimer(150);

            this.driven.OnUpdate += this.FullUpdate;
            this.driven.OnWrite += this.WritePosition;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.OnUpdate -= this.FullUpdate;
            this.driven.OnWrite -= this.WritePosition;
        }

        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            this.driven.OnUpdate += this.MinimumUpdate;
        }

        protected override void DisposeMinimum()
        {
            base.DisposeMinimum();

            this.driven.OnUpdate -= this.MinimumUpdate;
        }
        #endregion

        #region Frame Methods
        private void FullUpdate(GameTime gameTime)
        {
            _timer.Update(gameTime, t => t && this.driven.Fixtures.Any(), () =>
            { // Broadcast a positional upddate
                this.WritePosition(this.driven.Actions.Create(NetDeliveryMethod.UnreliableSequenced, 8));
            });
        }

        private void MinimumUpdate(GameTime gameTime)
        {
            if (this.driven.Authorization.HasFlag(GameAuthorization.Minimum))
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
        private void WritePosition(NetOutgoingMessage om)
            => BodyEntityNetworkDriver.WritePosition(this.driven, om);

        private void ReadPosition(NetIncomingMessage im)
        {
            this.driven.master.Position = im.ReadVector2();
            this.driven.master.Rotation = im.ReadSingle();

            this.driven.master.LinearVelocity = im.ReadVector2();
            this.driven.master.AngularVelocity = im.ReadSingle();
        }

        private void SkipPosition(NetIncomingMessage im)
            => im.Position += 192;
        #endregion

        #region Static Methods
        public static void WritePosition(BodyEntity entity, NetOutgoingMessage om)
        {
            om.Write("update:position", m =>
            {
                m.Write(entity.Position);
                m.Write(entity.Rotation);

                m.Write(entity.LinearVelocity);
                m.Write(entity.AngularVelocity);
            });
        }
        #endregion
    }
}
