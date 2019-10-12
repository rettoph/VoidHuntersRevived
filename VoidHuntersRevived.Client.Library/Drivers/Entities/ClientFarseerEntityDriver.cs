using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Extensions.Farseer;
using GalacticFighters.Library.Structs;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity), 90)]
    internal sealed class ClientFarseerEntityDriver : Driver<FarseerEntity>
    {
        #region Static Attributes
        /// <summary>
        /// Lerp strength per millisecond
        /// </summary>
        public static Single LerpStrength { get; set; } = 0.01f;
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
            this.driven.Events.TryAdd<Boolean>("farseer-enabled:changed", this.HandleFarseerEnabledChanged);
            this.driven.Events.TryAdd<Vector2>("linear-impulse:applied", this.HandleLinearImpulseApplied);
            this.driven.Events.TryAdd<Single>("angular-impulse:applied", this.HandleAngularImpulseApplied);
            this.driven.Events.TryAdd<AppliedForce>("force:applied", this.HandleForceApplied);
            this.driven.Events.TryAdd<Category>("collides-with:changed", this.HandleCollidesWithChanged);
            this.driven.Events.TryAdd<Category>("collision-categories:changed", this.HandleCollisionCategoriesChanged);
            this.driven.Events.TryAdd<NetIncomingMessage>("read", this.HandleRead);

            // Bind required action handlers
            this.driven.Actions.TryAdd("vitals:update", this.HandleVitalsUpdateMessage);
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

            if (this.driven.BodyEnabled)
            {
                var lerp = ClientFarseerEntityDriver.LerpStrength * (Single)gameTime.ElapsedGameTime.TotalMilliseconds;
                this.driven.SetPosition(
                    position: Vector2.Lerp(this.driven.Position, _body.Position, lerp),
                    rotation: MathHelper.Lerp(this.driven.Rotation, _body.Rotation, lerp));

                this.driven.SetVelocity(
                    linear: Vector2.Lerp(this.driven.LinearVelocity, _body.LinearVelocity, lerp),
                    angular: MathHelper.Lerp(this.driven.AngularVelocity, _body.AngularVelocity, lerp));
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the entities main body is created, we must clone a duplicate on the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="body"></param>
        private void HandleBodyCreated(object sender, Body body)
        {
            // Create a clone of the farseer entities body within the server render
            _body = _server.CloneBody(body);
            _body.UserData = body;
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
            _server.CloneFixture(fixture);
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
            _body.SetTransform(this.driven.Position, this.driven.Rotation);
        }

        private void HandleCollidesWithChanged(object sender, Category arg)
        {
            _body.CollidesWith = arg;
        }

        private void HandleCollisionCategoriesChanged(object sender, Category arg)
        {
            _body.CollisionCategories = arg;
        }
        private void HandleFarseerEnabledChanged(object sender, bool arg)
        {
            _body.Enabled = arg;
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
                    rotation: _body.Rotation);

                this.driven.SetVelocity(
                    linear: _body.LinearVelocity,
                    angular: _body.AngularVelocity);
            }
        }
        #endregion
    }
}
