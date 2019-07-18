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

            _player.AddActionHandler("set:ship", this.HandleSetShipAction);
            _player.AddActionHandler("set:direction", this.HandleSetShipDirection);
        }
        #endregion

        #region Action Handlers
        private void HandleSetShipAction(NetIncomingMessage obj)
        {
            _player.ReadShipData(obj);
        }

        private void HandleSetShipDirection(NetIncomingMessage obj)
        {
            // Read any incoming ship direction data
            _player.Ship.ReadDirectionData(obj);
        }
        #endregion
    }
}
