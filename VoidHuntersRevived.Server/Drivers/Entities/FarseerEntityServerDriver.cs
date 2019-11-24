using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Extensions.Farseer;
using VoidHuntersRevived.Library.Entities.Controllers;
using Microsoft.Extensions.Logging;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    /// <summary>
    /// This driver will primarily manage the farseer entity
    /// vital pings. These are actions containing required
    /// positional & rotational updates for the clients to 
    /// replicate
    /// </summary>
    [IsDriver(typeof(FarseerEntity))]
    internal sealed class FarseerEntityServerDriver : Driver<FarseerEntity>
    {
        #region Static Properties
        private static Double VitalsPingRate { get; set; } = 150;
        #endregion

        #region Private Fields
        private Double _lastVitalPing;
        #endregion

        #region Constructor
        public FarseerEntityServerDriver(FarseerEntity driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize()
        {
            base.PreInitialize();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Increase the ping tracker
            _lastVitalPing += gameTime.ElapsedGameTime.TotalMilliseconds; 

            if(this.driven.Body.IsSolidEnabled() && ((this.driven.Body.Awake && _lastVitalPing >= FarseerEntityServerDriver.VitalsPingRate) || this.driven.Controller is Chunk))
            { // Only bother sending vital pings if the body is awake & there are any fixtures...
                var action = this.driven.Actions.Create("update:vitals", NetDeliveryMethod.UnreliableSequenced, 2);
                this.driven.Body.WriteVitals(action);

                // Reset the ping tracker
                _lastVitalPing %= FarseerEntityServerDriver.VitalsPingRate;
            }
        }
        #endregion
    }
}
