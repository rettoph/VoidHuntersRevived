using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.Players;

namespace VoidHuntersRevived.Client.Library.Drivers.Entities.Players
{
    [IsDriver(typeof(Player))]
    internal sealed class PlayerClientDriver : Driver<Player>
    {
        #region Constructor
        public PlayerClientDriver(Player driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.Actions.TryAdd("team:changed", this.HandleTeamChangedAction);
        }


        #endregion

        #region Action Handlers
        private void HandleTeamChangedAction(object sender, NetIncomingMessage arg)
        {
            this.driven.ReadTeam(arg);
        }
        #endregion
    }
}
