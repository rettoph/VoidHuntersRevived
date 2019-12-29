using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Server.Drivers.Entities.Players
{
    [IsDriver(typeof(Player))]
    internal sealed class PlayerServerDriver : Driver<Player>
    {
        #region Constructor
        public PlayerServerDriver(Player driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.driven.OnTeamChanged += this.HandleTeamChanged;
        }
        #endregion

        #region Event Handlers
        private void HandleTeamChanged(object sender, Team e)
        {
            var action = this.driven.Actions.Create("team:changed", NetDeliveryMethod.ReliableOrdered, 5);
            this.driven.WriteTeam(action);
        }
        #endregion
    }
}
