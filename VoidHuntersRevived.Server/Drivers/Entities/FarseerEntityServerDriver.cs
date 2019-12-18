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
using VoidHuntersRevived.Library.Utilities;

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

        #region Constructor
        public FarseerEntityServerDriver(FarseerEntity driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.WriteBodyVitals += this.WriteBodyVitals;
        }
        #endregion

        #region Event Handlers
        private void WriteBodyVitals(object sender, NetOutgoingMessage om)
        {
            this.driven.Body.WriteVitals(om);
        }
        #endregion
    }
}
