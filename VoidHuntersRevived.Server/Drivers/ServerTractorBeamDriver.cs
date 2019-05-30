using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerTractorBeamDriver : Driver
    {
        private TractorBeam _tractorBeam;

        public ServerTractorBeamDriver(TractorBeam tractorBeam, IServiceProvider provider, ILogger logger) : base(tractorBeam, provider, logger)
        {
            _tractorBeam = tractorBeam;
        }

        protected override void Boot()
        {
            base.Boot();

            _tractorBeam.OnOffsetChanged += this.HandleOffsetChanged;
        }

        #region Event Handlers
        private void HandleOffsetChanged(object sender, Vector2 e)
        {
            var action = _tractorBeam.CreateActionMessage("update:offset");
            action.Write(_tractorBeam.Offset);
        }
        #endregion
    }
}
