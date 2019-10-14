using FarseerPhysics.Dynamics;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Utilities.Delegater;
using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    [IsDriver(typeof(FarseerEntity))]
    public sealed class ServerFarseerEntityDriver : Driver<FarseerEntity>
    {
        #region Static Attributes
        public static Double UpdateVitalsRate { get; set; } = 120;
        #endregion

        #region Private Fields
        private Body _body;
        private Boolean _dirtyVitals;
        private Vector2 _flushedPosition;
        private Interval _interval;
        #endregion

        #region Constructor
        public ServerFarseerEntityDriver(Interval intervals, FarseerEntity driven) : base(driven)
        {
            _interval = intervals;
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();

            // Register an event to store the driven's body when created.
            this.driven.Events.TryAdd<Body>("body:created", (s, b) => _body = b);
            this.driven.Events.TryAdd<Boolean>("reserved:changed", (s, arg) => _dirtyVitals = true);
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.CanSendVitals())
                this.SendVitals();
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
            if (this.driven.FixtureCount == 0)
                return false;

            // Instant yes
            if ((_interval.Is(ServerFarseerEntityDriver.UpdateVitalsRate) && Vector2.Distance(_body.Position, _flushedPosition) > 0.1f) || _dirtyVitals)
                return true;

            // Default to no
            return false;
        }

        private void SendVitals()
        {
            if (this.CanSendVitals())
            { // Send the vitals data to all connected clients
                var om = this.driven.Actions.Create("vitals:update", NetDeliveryMethod.Unreliable, 3);
                // Write the vitals data
                _body.WritePosition(om);
                _body.WriteVelocity(om);

                if (_dirtyVitals) // If the entity has dirty vitals, mark them as clean
                    _dirtyVitals = false;

                _flushedPosition = _body.Position;
            }
        }
        #endregion
    }
}
