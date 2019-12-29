using Guppy;
using Guppy.Attributes;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.Players;
using Guppy.Network.Extensions.Lidgren;

namespace VoidHuntersRevived.Server.Drivers.Entities
{
    [IsDriver(typeof(Team))]
    internal sealed class TeamServerDriver : Driver<Team>
    {
        #region Constructor
        public TeamServerDriver(Team driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize()
        {
            base.Initialize();
        }
        #endregion
    }
}
