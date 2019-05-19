using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Drivers;
using VoidHuntersRevived.Library.Structs;

namespace VoidHuntersRevived.Client.Library.Entities.Drivers
{
    public class ClientFarseerEntityDriver : FarseerEntityDriver
    {
        /// <summary>
        /// Wether or not the entity's body was awake last frame
        /// </summary>
        private Boolean _wasAwake;

        /// <summary>
        /// The servers claimed body info
        /// </summary>
        private BodyInfo _serverBodyInfo;

        /// <summary>
        /// The client body to server body lerp strength
        /// </summary>
        private Single _clientOffsetLerpStrength;

        /// <summary>
        /// The client to server ratio threshold 
        /// that must be passed to snap the client to
        /// server claim.
        /// </summary>
        private Single _clientOffsetSnapThreshold;

        public ClientFarseerEntityDriver(FarseerEntity parent, EntityConfiguration configuration, Scene scene, ILogger logger) : base(parent, configuration, scene, logger)
        {
        }

        protected override void Boot()
        {
            base.Boot();

            _clientOffsetSnapThreshold = 5;
            _clientOffsetLerpStrength = 0.1f;
            _serverBodyInfo = new BodyInfo(this.parent.Body);

            this.parent.ActionHandlers.Add("update:body-info", this.HandleUpdateBodyInfoAction);
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (this.parent.Body.Awake)
            {
                _serverBodyInfo.LinearVelocity = this.parent.Body.LinearVelocity;
                _serverBodyInfo.AngularVelocity = this.parent.Body.AngularVelocity;

                _serverBodyInfo.Update(gameTime);

                // Calculate the offsets between the current client position/rotation and the server position/rotation.
                var clientPositionDifference = this.parent.Body.Position.Length() - _serverBodyInfo.Position.Length();
                var clientRotationDifference = Math.Abs(this.parent.Body.Rotation - _serverBodyInfo.Rotation);


                
                if (clientPositionDifference / _serverBodyInfo.LinearVelocity.Length() > _clientOffsetSnapThreshold)
                { // If the client position offset is too much, snap to what the server position claim
                    this.logger.LogWarning($"Snapping FarseerEntity<{this.parent.GetType().Name}>({this.parent.Id}) to server position!");
                    this.parent.Body.Position = _serverBodyInfo.Position;
                }
                else
                { // Otherwise lerp to the server position claim
                    this.parent.Body.Position = Vector2.Lerp(this.parent.Body.Position, _serverBodyInfo.Position, _clientOffsetLerpStrength);
                }

                // If the client rotation offset is too much, snap to what the server position claim
                if (clientRotationDifference / _serverBodyInfo.AngularVelocity > _clientOffsetSnapThreshold)
                    this.parent.Body.Rotation = _serverBodyInfo.Rotation;
                else // Otherwise lerp to the server rotation claim
                    this.parent.Body.Rotation = MathHelper.Lerp(this.parent.Body.Rotation, _serverBodyInfo.Rotation, _clientOffsetLerpStrength);

                _wasAwake = true;
            }
            else if (_wasAwake)
            {
                this.SnapToServer();

                _wasAwake = false;
            }
        }

        /// <summary>
        /// Instantly snap the parent object to the server position/velocity
        /// </summary>
        private void SnapToServer()
        {
            this.parent.Body.Position = _serverBodyInfo.Position;
            this.parent.Body.Rotation = _serverBodyInfo.Rotation;

            this.parent.Body.LinearVelocity = _serverBodyInfo.LinearVelocity;
            this.parent.Body.AngularVelocity = _serverBodyInfo.AngularVelocity;
        }

        #region Action Handlers
        /// <summary>
        /// When a body update method is recieved from the server,
        /// update the farseer objects body data
        /// </summary>
        /// <param name="obj"></param>
        private void HandleUpdateBodyInfoAction(NetIncomingMessage obj)
        {
            _serverBodyInfo.Read(obj);

            if (obj.ReadBoolean())
                this.SnapToServer();

            this.parent.Body.LinearVelocity = _serverBodyInfo.LinearVelocity;
            this.parent.Body.AngularVelocity = _serverBodyInfo.AngularVelocity;
        }
        #endregion
    }
}
