using Guppy.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.CustomEventArgs;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerPlayerDriver : Driver
    {
        private Player _player;

        #region Constructors
        public ServerPlayerDriver(Player parent, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.OnShipChanged += this.HandleShipChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleShipChanged(object sender, ChangedEventArgs<Ship> e)
        {
            var action = _player.CreateActionMessage("set:ship");
            _player.WriteShipData(action);
        }
        #endregion
    }
}
