using FarseerPhysics.Dynamics;
using GalacticFighters.Library.Entities;
using GalacticFighters.Library.Extensions.Farseer;
using Guppy;
using Guppy.Attributes;
using Microsoft.Extensions.Logging;
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
        public static Double UpdateVitalsRate { get; set; } = 100;
        #endregion

        #region Private Fields
        private Double _lastUpdateVitals;
        private Body _body;
        private Boolean _dirtyVitals;
        private Boolean _wasAwake;
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
            this.driven.Events.TryAdd<Boolean>("reserved:changed", (s, arg) => _dirtyVitals = true);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _lastUpdateVitals += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.CanSendVitals())
            { // Send the vitals data to all connected clients
                var om = this.driven.Actions.Create("vitals:update");
                // Write the vitals data
                _body.WritePosition(om);
                _body.WriteVelocity(om);

                _lastUpdateVitals = _lastUpdateVitals % ServerFarseerEntityDriver.UpdateVitalsRate;

                if (_dirtyVitals) // If the entity has dirty vitals, mark them as clean
                    _dirtyVitals = false;
            }

            // Update the internal was awake value
            _wasAwake = this.driven.Awake;
        }
        #endregion

        #region Methods
        private Boolean CanSendVitals()
        {
            // Instant no
            if (this.driven.Reserverd.Value)
                return false;
            if (!this.driven.BodyEnabled)
                return false;

            // Instant yes
            if (_lastUpdateVitals > ServerFarseerEntityDriver.UpdateVitalsRate || _dirtyVitals)
                return true;
            if (!this.driven.Awake && _wasAwake)
                return true;

            // Default to no
            return false;
        }
        #endregion
    }
}
