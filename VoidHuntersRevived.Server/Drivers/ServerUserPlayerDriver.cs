using Guppy.Implementations;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Server.Drivers
{
    public class ServerUserPlayerDriver : Driver
    {
        private UserPlayer _player;

        #region Constructors
        public ServerUserPlayerDriver(UserPlayer parent, IServiceProvider provider) : base(parent, provider)
        {
            _player = parent;
        }
        #endregion

        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            _player.AddActionHandler("set:direction", this.HandleSetDirectionAction);
        }
        #endregion

        #region Action Handlers
        private void HandleSetDirectionAction(NetIncomingMessage obj)
        {
            // Read any incoming direction update data
            _player.Ship.ReadDirectionData(obj);
        }
        #endregion
    }
}
