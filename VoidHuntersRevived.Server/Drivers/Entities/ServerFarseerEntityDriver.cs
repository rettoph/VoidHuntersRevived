using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Extensions.Farseer;
using Guppy;
using Guppy.Attributes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace GalacticFighters.Server.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity))]
    public sealed class ServerFarseerEntityDriver : Driver<FarseerEntity>
    {
        #region Static Attributes
        public static Double UpdateVitalsRate { get; set; } = 1000;
        #endregion

        #region Private Fields
        private Double _lastUpdateVitals;
        private Body _body;
        #endregion

        #region Constructor
        public ServerFarseerEntityDriver(FarseerEntity driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            _lastUpdateVitals = 0;

            // Register an event to store the driven's body when created.
            this.driven.Events.TryAdd<Body>("body:created", (s, b) => _body = b);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _lastUpdateVitals += gameTime.ElapsedGameTime.TotalMilliseconds;

            if(_lastUpdateVitals >= ServerFarseerEntityDriver.UpdateVitalsRate)
            { // Send the vitals data to all connected clients
                var om = this.driven.Actions.Create("vitals:update");
                // Write the vitals data
                _body.WritePosition(om);
                _body.WriteVelocity(om);

                _lastUpdateVitals = _lastUpdateVitals % ServerFarseerEntityDriver.UpdateVitalsRate;
            }
        }
        #endregion
    }
}
