﻿using FarseerPhysics.Dynamics;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Client.Library.Utilities;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Extensions.Farseer;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity))]
    internal sealed class FarseerEntityClientDriver : Driver<FarseerEntity>
    {
        #region Static Properties
        private static Single VitalsLerpStrength { get; set; } = 0.001625f;
        private static Single PositionSnapThreshold { get; set; } = 5f;
        private static Single RotationSnapThreshold { get; set; } = MathHelper.PiOver2;
        #endregion

        #region Private Fields
        private ServerShadow _server;
        private Body _shadow;
        private Controller _controller;
        #endregion

        #region Constructor
        public FarseerEntityClientDriver(ServerShadow server, FarseerEntity driven) : base(driven)
        {
            _server = server;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _shadow = this.driven.CreateBody(_server.World);

            this.driven.ReadBodyVitals += this.ReadBodyVitals;
            this.driven.OnControllerChanged += this.HandleControllerChanged;
        }

        protected override void Initialize()
        {
            base.Initialize();

            _shadow.LinearVelocity = this.driven.Body.LinearVelocity;
            _shadow.AngularVelocity = this.driven.Body.AngularVelocity;
            _shadow.SetTransformIgnoreContacts(
                position: this.driven.Body.Position,
                angle: this.driven.Body.Rotation);
        }

        protected override void Dispose()
        {
            base.Dispose();

            _shadow.Dispose(withFixtures: true);

            this.driven.OnControllerChanged -= this.HandleControllerChanged;
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.driven.Body.IsSolidEnabled())
            { // If there are any fixtures within the body, attempt to lerp towards the server shadow
                this.driven.Controller.UpdateBody(this.driven, _shadow);

                var lerp = FarseerEntityClientDriver.VitalsLerpStrength * (Single)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (this.driven.Controller is Chunk || MathHelper.Distance(this.driven.Rotation, _shadow.Rotation) > FarseerEntityClientDriver.RotationSnapThreshold || Vector2.Distance(this.driven.Position, _shadow.Position) > FarseerEntityClientDriver.PositionSnapThreshold)
                    this.driven.Body.SetTransformIgnoreContacts(
                        position: _shadow.Position, 
                        angle: _shadow.Rotation);
                else
                    this.driven.Body.SetTransformIgnoreContacts(
                        position: Vector2.Lerp(this.driven.Position, _shadow.Position, lerp),
                        angle: MathHelper.Lerp(this.driven.Rotation, _shadow.Rotation, lerp));

                // Update the velocity
                this.driven.Body.LinearVelocity = Vector2.Lerp(this.driven.LinearVelocity, _shadow.LinearVelocity, lerp);
                this.driven.Body.AngularVelocity = MathHelper.Lerp(this.driven.AngularVelocity, _shadow.AngularVelocity, lerp);
            }
        }
        #endregion

        #region Event Handler
        private void HandleControllerChanged(object sender, Controller arg)
        {
            if(_controller != default(Controller))
            { // Invoke the entities setdown method
                _controller.UpdateBody(this.driven, _shadow);
                _controller = arg;
            }
            // Auto setup the shadow body
            arg?.SetupBody(this.driven, _shadow);
        }
        #endregion

        #region Action Handlers
        private void ReadBodyVitals(object sender, NetIncomingMessage im)
        {
            _shadow.ReadVitals(im);
        }
        #endregion
    }
}
