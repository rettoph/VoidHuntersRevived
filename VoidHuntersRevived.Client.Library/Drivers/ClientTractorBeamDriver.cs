using Guppy.Implementations;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    class ClientTractorBeamDriver : Driver
    {
        private TractorBeam _tractorBeam;

        public ClientTractorBeamDriver(TractorBeam tractorBeam, IServiceProvider provider, ILogger logger) : base(tractorBeam, provider, logger)
        {
            _tractorBeam = tractorBeam;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            _tractorBeam.ActionHandlers["update:offset"] = this.HandleUpdateOffsetAction;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_tractorBeam.Player.Bridge != null)
                ClientFarseerEntityDriver.ServerBody[_tractorBeam].Position = _tractorBeam.Player.Bridge.WorldCenter + _tractorBeam.Offset;
        }

        #region Action Handlers
        private void HandleUpdateOffsetAction(NetIncomingMessage obj)
        {
            _tractorBeam.SetOffset(obj.ReadVector2());
        }
        #endregion
    }
}
