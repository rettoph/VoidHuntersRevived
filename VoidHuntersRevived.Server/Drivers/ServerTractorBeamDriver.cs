﻿using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.ShipParts;

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
            _tractorBeam.OnSelected += this.HandleSelected;
            _tractorBeam.OnReleased += this.HandleReleased;
        }

        #region Event Handlers
        private void HandleOffsetChanged(object sender, Vector2 e)
        {
            var action = _tractorBeam.CreateActionMessage("update:offset");
            action.Write(_tractorBeam.Offset);
        }

        private void HandleSelected(object sender, ShipPart e)
        {
            var action = _tractorBeam.CreateActionMessage("select");
            action.Write(e.Id);
        }

        private void HandleReleased(object sender, ShipPart e)
        {
            var action = _tractorBeam.CreateActionMessage("release");
            action.Write(e.Id);
        }
        #endregion
    }
}
