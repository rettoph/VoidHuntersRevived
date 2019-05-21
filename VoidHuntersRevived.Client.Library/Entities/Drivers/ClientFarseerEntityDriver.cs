using System;
using System.Collections.Generic;
using System.Text;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Guppy;
using Guppy.Configurations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Drivers;

namespace VoidHuntersRevived.Client.Library.Entities.Drivers
{
    public class ClientFarseerEntityDriver : FarseerEntityDriver
    {
        /// <summary>
        /// Wether or not the entity's body was awake last frame
        /// </summary>
        private Boolean _wasAwake;

        /// <summary>
        /// The client body to server body lerp strength
        /// </summary>
        private Single _clientOffsetLerpStrength;

        /// <summary>
        /// The client to server ratio threshold 
        /// that must be passed to snap the client to
        /// server claim.
        /// </summary>
        private Single _clientPositionOffsetSnapThreshold;

        /// <summary>
        /// The client to server ratio threshold 
        /// that must be passed to snap the client to
        /// server claim.
        /// </summary>
        private Single _clientRotationOffsetSnapThreshold;

        private Body _serverBody;

        public ClientFarseerEntityDriver(FarseerEntity parent, EntityConfiguration configuration, Scene scene, ILogger logger) : base(parent, configuration, scene, logger)
        {
        }

        protected override void Boot()
        {
            base.Boot();

            _clientPositionOffsetSnapThreshold = 0.2f;
            _clientRotationOffsetSnapThreshold = 0.436332f;
            _clientOffsetLerpStrength = 0.1f;
            _serverBody = BodyFactory.CreateBody((this.scene as VoidHuntersClientWorldScene).ServerWorld, this.parent.Body.Position, this.parent.Body.Rotation, this.parent.Body.BodyType);
            _serverBody.AngularDamping = 1f;
            _serverBody.LinearDamping = 1f;

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
                // Calculate the offsets between the current client position/rotation and the server position/rotation.
                var clientPositionDifference = (this.parent.Body.Position - _serverBody.Position).Length();
                var clientRotationDifference = Math.Abs(this.parent.Body.Rotation - _serverBody.Rotation);


                
                if (clientPositionDifference > _clientPositionOffsetSnapThreshold)
                { // If the client position offset is too much, snap to what the server position claim
                    this.logger.LogWarning($"Snapping FarseerEntity<{this.parent.GetType().Name}>({this.parent.Id}) to server position!");
                    this.parent.Body.Position = _serverBody.Position;
                }
                else
                { // Otherwise lerp to the server position claim
                    this.parent.Body.Position = Vector2.Lerp(this.parent.Body.Position, _serverBody.Position, _clientOffsetLerpStrength);
                }

                // If the client rotation offset is too much, snap to what the server position claim
                if (clientRotationDifference > _clientRotationOffsetSnapThreshold)
                {
                    this.logger.LogWarning($"Snapping FarseerEntity<{this.parent.GetType().Name}>({this.parent.Id}) to server rotation!");

                    this.parent.Body.Rotation = _serverBody.Rotation;
                }
                else // Otherwise lerp to the server rotation claim
                    this.parent.Body.Rotation = MathHelper.Lerp(this.parent.Body.Rotation, _serverBody.Rotation, _clientOffsetLerpStrength);

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
            this.parent.Body.Position = _serverBody.Position;
            this.parent.Body.Rotation = _serverBody.Rotation;

            this.parent.Body.LinearVelocity = _serverBody.LinearVelocity;
            this.parent.Body.AngularVelocity = _serverBody.AngularVelocity;
        }

        #region Action Handlers
        /// <summary>
        /// When a body update method is recieved from the server,
        /// update the farseer objects body data
        /// </summary>
        /// <param name="obj"></param>
        private void HandleUpdateBodyInfoAction(NetIncomingMessage obj)
        {
            _serverBody.Position = obj.ReadVector2();
            _serverBody.Rotation = obj.ReadSingle();
            _serverBody.LinearVelocity = obj.ReadVector2();
            _serverBody.AngularVelocity = obj.ReadSingle();

            if (obj.ReadBoolean())
                this.SnapToServer();
        }
        #endregion

        #region Farseer Methods
        public override void ApplyLinearImpulse(Vector2 impulse)
        {
            base.ApplyLinearImpulse(impulse);

            _serverBody.ApplyLinearImpulse(impulse);
        }

        public override void ApplyAngularImpulse(Single impulse)
        {
            base.ApplyAngularImpulse(impulse);

            _serverBody.ApplyAngularImpulse(impulse);
        }

        public override void CreateFixture(Shape shape)
        {
            base.CreateFixture(shape);

            _serverBody.CreateFixture(shape.Clone(), this);
        }
        #endregion
    }
}
