using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Client.Library.Scenes;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientFarseerEntityDriver : Driver
    {
        private Body _serverBody;
        private VoidHuntersClientWorldScene _scene;
        private FarseerEntity _entity;
        private Single _lerpStrength;

        #region Constructors
        public ClientFarseerEntityDriver(VoidHuntersClientWorldScene scene, FarseerEntity entity, IServiceProvider provider, ILogger logger) : base(entity, provider, logger)
        {
            _scene = scene;
            _entity = entity;
        }
        #endregion

        #region Initialization Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind action handlers
            _entity.ActionHandlers.Add("update:position", this.HandleUpdatePositionAction);

            // Bind event handlers
            _entity.OnFixtureCreated += this.HandleFixtureCreated;
            _entity.OnLinearImpulseApplied += this.HandleLinearImpulseApplied;
            _entity.OnAngularImpulseApplied += this.HandleAngularImpulseApplied;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _lerpStrength = 0.05f;

            // Create a new body within the server world to represent the server render of the current entity
            _serverBody = _entity.CreateBody(_scene.ServerWorld, _entity.Body.Position, _entity.Body.Rotation);
        }
        #endregion

        #region Frame Methods
        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if(_serverBody.Awake)
            {
                _entity.Body.Position = Vector2.Lerp(_entity.Body.Position, _serverBody.Position, _lerpStrength);
                _entity.Body.Rotation = MathHelper.Lerp(_entity.Body.Rotation, _serverBody.Rotation, _lerpStrength);
                _entity.Body.LinearVelocity = Vector2.Lerp(_entity.Body.LinearVelocity, _serverBody.LinearVelocity, _lerpStrength);
                _entity.Body.AngularVelocity = MathHelper.Lerp(_entity.Body.AngularVelocity, _serverBody.AngularVelocity, _lerpStrength);
            }
        }
        #endregion

        #region Action Handlers
        private void HandleUpdatePositionAction(NetIncomingMessage obj)
        {
            _serverBody.Position = obj.ReadVector2();
            _serverBody.Rotation = obj.ReadSingle();
            _serverBody.LinearVelocity = obj.ReadVector2();
            _serverBody.AngularVelocity = obj.ReadSingle();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the client render recieves a fixture,
        /// buplocate it on the server render too
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shape"></param>
        private void HandleFixtureCreated(object sender, Shape shape)
        {
            _serverBody.CreateFixture(shape);
        }

        private void HandleAngularImpulseApplied(object sender, float e)
        {
            _serverBody.ApplyAngularImpulse(e);
        }

        private void HandleLinearImpulseApplied(object sender, Vector2 e)
        {
            _serverBody.ApplyLinearImpulse(e);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _serverBody.Dispose();
        }
    }
}
