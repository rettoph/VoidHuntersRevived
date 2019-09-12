using FarseerPhysics.Dynamics;
using GalacticFighters.Client.Library.Utilities;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Extensions.Farseer;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
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
        public static Single LerpStrength { get; set; } = 0.1f;
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
            this.driven.Events.TryAdd<Vector2>("linear-impulse:applied", this.HandleLinearImpulseApplied);
            this.driven.Events.TryAdd<Single>("angular-impulse:applied", this.HandleAngularImpulseApplied);
            this.driven.Events.TryAdd<NetIncomingMessage>("on:read", this.HandleRead);

            // Bind required action handlers
            this.driven.Actions.TryAdd("vitals:update", this.HandleVitalsUpdateMessage);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.driven.SetPosition(
                position: Vector2.Lerp(this.driven.Position, _body.Position, ClientFarseerEntityDriver.LerpStrength),
                rotation: MathHelper.Lerp(this.driven.Rotation, _body.Rotation, ClientFarseerEntityDriver.LerpStrength));

            this.driven.SetVelocity(
                linear: Vector2.Lerp(this.driven.LinearVelocity, _body.LinearVelocity, ClientFarseerEntityDriver.LerpStrength),
                angular: MathHelper.Lerp(this.driven.AngularVelocity, _body.AngularVelocity, ClientFarseerEntityDriver.LerpStrength));
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
        /// When a lngular impulse is applied, we must simulate the very same impulse on
        /// the server render
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="impulse"></param>
        private void HandleLinearImpulseApplied(object sender, Vector2 impulse)
        {
            _body.ApplyLinearImpulse(impulse);
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
        #endregion

        #region Action Handlers
        private void HandleVitalsUpdateMessage(object sender, NetIncomingMessage arg)
        {
            _body.ReadPosition(arg);
            _body.ReadVelocity(arg);

            this.driven.SetEnabled(true);
        }
        #endregion
    }
}
