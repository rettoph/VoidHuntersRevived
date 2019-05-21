using System;
using System.Collections.Generic;
using System.Text;
using Guppy;
using Guppy.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Drivers;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Entities.Drivers
{
    class ServerFarseerEntityDriver : FarseerEntityDriver
    {
        /// <summary>
        /// The time elapsed since the last time the body info
        /// was flushed to clients
        /// </summary>
        private Double _timeWhenFlushed;

        /// <summary>
        /// Simple bool used to track if the body was awake
        /// during the last update.
        /// </summary>
        private Boolean _wasAwake;

        /// <summary>
        /// The amount of time to wait between server body info flushing.
        /// </summary>
        private Double _timeToFlush;

        public ServerFarseerEntityDriver(FarseerEntity parent, EntityConfiguration configuration, Scene scene, ILogger logger) : base(parent, configuration, scene, logger)
        {
        }

        protected override void Boot()
        {
            base.Boot();

            _timeToFlush = 100;
        }

        public override void Draw(GameTime gameTime)
        {
            // throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            if (this.parent.Body.Awake)
            {
                if(!_wasAwake || gameTime.TotalGameTime.TotalMilliseconds - _timeWhenFlushed >= _timeToFlush)
                { // If the body was just asleep or enough time has passed since the last flush...
                    // Flush the server body info
                    this.FlushBodyInfo(false);
                    _timeWhenFlushed = gameTime.TotalGameTime.TotalMilliseconds;
                }

                _wasAwake = true;
            }
            else if (_wasAwake)
            { // When the body sleeps, ensure that we flush the full body data to clients.
                this.FlushBodyInfo(true);
                _timeWhenFlushed = gameTime.TotalGameTime.TotalMilliseconds;

                _wasAwake = false;
            }
        }

        /// <summary>
        /// Flush the body to all connected clients.
        /// </summary>
        /// <param name="snapTo">Should the clients instantly snap to this position.</param>
        private void FlushBodyInfo(Boolean snapTo = false)
        {
            // Create a new action method for the body data...
            var action = this.parent.CreateActionMessage("update:body-info");
            action.Write(this.parent.Body.Position);
            action.Write(this.parent.Body.Rotation);
            action.Write(this.parent.Body.LinearVelocity);
            action.Write(this.parent.Body.AngularVelocity);
            action.Write(snapTo);
        }
    }
}
