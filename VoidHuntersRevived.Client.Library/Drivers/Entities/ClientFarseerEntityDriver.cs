using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Structs;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities.Controllers;
using System.Linq;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity), 90)]
    internal sealed class ClientFarseerEntityDriver : Driver<FarseerEntity>
    {
        #region Static Attributes
        /// <summary>
        /// Lerp strength per millisecond
        /// </summary>
        public static Single LerpStrength { get; set; } = 0.001f;
        #endregion

        #region Private Fields
        private ServerRender _server;
        private Body _body;
        #endregion

        #region Constructor
        public ClientFarseerEntityDriver(ServerRender server, FarseerEntity driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Bind required events...
            this.driven.Events.TryAdd<Body>("body:created", this.HandleBodyCreated);
            this.driven.Events.TryAdd<Body>("body:destroyed", this.HandleBodyDestroyed);
            this.driven.Events.TryAdd<Fixture>("fixture:created", this.HandleFixtureCreated);
            this.driven.Events.TryAdd<Fixture>("fixture:destroyed", this.HandleFixtureDestroyed);
            this.driven.Events.TryAdd<Boolean>("body-enabled:changed", this.HandleBodyEnabledChanged);
            this.driven.Events.TryAdd<Vector2>("linear-impulse:applied", this.HandleLinearImpulseApplied);
            this.driven.Events.TryAdd<Single>("angular-impulse:applied", this.HandleAngularImpulseApplied);
            this.driven.Events.TryAdd<AppliedForce>("force:applied", this.HandleForceApplied);
            this.driven.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);
            this.driven.Events.TryAdd<Controller>("controller:changed", this.HandleControllerChanged);

            // Bind required action handlers
            this.driven.Actions.TryAdd("vitals:update", this.HandleVitalsUpdateMessage);
        }

        protected override void PostInitialize()
        {
            base.PostInitialize();
        }

        protected override void Dispose()
        {
            base.Dispose();

            _server.DestroyBody(_body.UserData as Body);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.BodyEnabled && _body.FixtureList.Any())
            {
                var lerp = ClientFarseerEntityDriver.LerpStrength * (Single)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (Vector2.Distance(this.driven.Position, _body.Position) > 5f) // Snap to the correct position if needed
                    this.driven.SetPosition(_body.Position, _body.Rotation, true);
                else
                    this.driven.SetPosition(
                        position: Vector2.Lerp(this.driven.Position, _body.Position, lerp),
                        rotation: MathHelper.Lerp(this.driven.Rotation, _body.Rotation, lerp),
                        ignoreContacts: true);

                this.driven.SetVelocity(
                    linear: Vector2.Lerp(this.driven.LinearVelocity, _body.LinearVelocity, lerp),
                    angular: MathHelper.Lerp(this.driven.AngularVelocity, _body.AngularVelocity, lerp));
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When a controller is updated, refresh the internal collision categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleControllerChanged(object sender, Controller arg)
        {
            if (_body != null && !_body.IsDisposed)
            {
                _body.CollidesWith = this.driven.CollidesWith;
                _body.CollisionCategories = this.driven.CollisionCategories;
                _body.IgnoreCCDWith = this.driven.IgnoreCCDWith;
                _body.LinearVelocity = Vector2.Zero;
                _body.AngularVelocity = 0;
            }
        }

        /// <summary>
        /// When the entities main body is created, we must clone a duplicate on the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="body"></param>
        private void HandleBodyCreated(object sender, Body body)
        {
            // Create a clone of the farseer entities body within the server render\
            _body = _server.CloneBody(body);
            _body.UserData = body;

            this.driven.SetBodyEnabled(false);
        }

        /// <summary>
        /// When the entities body is destroyed, we must destroy the clone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="body"></param>
        private void HandleBodyDestroyed(object sender, Body body)
        {
            // Destroy the server render for the driven entity
            _server.DestroyBody(body);
        }

        /// <summary>
        /// When a fixture is created, create a clone of it within the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fixture"></param>
        private void HandleFixtureCreated(object sender, Fixture fixture)
        {
            var fixtre = _server.CloneFixture(fixture);
        }

        /// <summary>
        /// When a fixture is destroyed, destroy its clone within the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="fixture"></param>
        private void HandleFixtureDestroyed(object sender, Fixture fixture)
        {
            _server.DestroyFixture(fixture);
        }

        /// <summary>
        /// When an angular impulse is applied, we must simulate the very same impulse on
        /// the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="impulse"></param>
        private void HandleAngularImpulseApplied(object sender, float impulse)
        {
            _body.ApplyAngularImpulse(impulse);
        }

        /// <summary>
        /// When a linear impulse is applied, we must simulate the very same impulse on
        /// the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="impulse"></param>
        private void HandleLinearImpulseApplied(object sender, Vector2 impulse)
        {
            _body.ApplyLinearImpulse(impulse);
        }

        /// <summary>
        /// When a force is applied at a point, we must similate the same force on the client render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleForceApplied(object sender, AppliedForce arg)
        {
            _body.ApplyForce(arg.Force, arg.Point);
        }

        /// <summary>
        /// When the client data is read, we must duplicate all
        /// of the raw positional data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleRead(object sender, NetIncomingMessage arg)
        {
            // Update the positional data...
            _body.SetTransformIgnoreContacts(this.driven.Position, this.driven.Rotation);
        }

        private void HandleBodyEnabledChanged(object sender, bool arg)
        {
            _body.Enabled = arg;

            this.driven.SetBodyEnabled(true);
        }
        #endregion

        #region Action Handlers
        private void HandleVitalsUpdateMessage(object sender, NetIncomingMessage arg)
        {
            _body.ReadPosition(arg);
            _body.ReadVelocity(arg);

            if (!this.driven.BodyEnabled)
            {
                this.driven.SetPosition(
                    position: _body.Position,
                    rotation: _body.Rotation,
                    ignoreContacts: true);

                this.driven.SetVelocity(
                    linear: _body.LinearVelocity,
                    angular: _body.AngularVelocity);
            }
        }
        #endregion
    }
}
