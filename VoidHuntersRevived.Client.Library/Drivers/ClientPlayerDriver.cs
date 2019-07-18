using Guppy.Implementations;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Drivers
{
    public class ClientPlayerDriver : Driver
    {
        private Player _player;

        #region Constructors
        public ClientPlayerDriver(Player parent, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.ActionHandlers["set:ship"] = this.HandleSetShipAction;
        }
        #endregion

        #region Action Handlers
        private void HandleSetShipAction(NetIncomingMessage obj)
        {
            _player.ReadShipData(obj);
        }
        #endregion
    }
}
