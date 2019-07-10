using Guppy.Implementations;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Entities.ShipParts;
using System.Linq;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerTractorBeamDriver : Driver
    {
        private TractorBeam _tractorBeam;

        public ServerTractorBeamDriver(TractorBeam tractorBeam, IServiceProvider provider) : base(tractorBeam, provider)
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
            if(e == null)
            { // Force a release on all clients
                var action = _tractorBeam.CreateActionMessage("release:force");
            }
            else
            { // Alert all clients that a piece was selected
                var action = _tractorBeam.CreateActionMessage("select");
                action.Write(e.Id);
            }
        }

        private void HandleReleased(object sender, ShipPart e)
        {
            var action = _tractorBeam.CreateActionMessage("release");
            action.Write(e.Id);

            // Attempt to attach the part to the ship.
            _tractorBeam.Player.TryAttach(e);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();

            _tractorBeam.OnSelected -= this.HandleSelected;
            _tractorBeam.OnReleased -= this.HandleReleased;
        }
    }
}
