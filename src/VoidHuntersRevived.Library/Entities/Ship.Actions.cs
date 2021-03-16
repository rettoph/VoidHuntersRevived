using Guppy.DependencyInjection;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Utilities;

namespace VoidHuntersRevived.Library.Entities
{
    /// <summary>
    /// Actions represent dynamic "events" that may be invoked
    /// within a ship. In an effort to manage future modding of 
    /// ship functionality there is no rigid definition for a ship.
    /// 
    /// Actions are organized by a unique UInt32 id, for which I 
    /// personally recommend a "handle".xxHash() constant.
    /// </summary>
    public partial class Ship
    {
        #region Public Properties
        public ActionManager<Ship> Actions { get; private set; }
        #endregion

        #region Lifecycle Methods
        private void Actions_Create(ServiceProvider provider)
        {
            this.Actions = new ActionManager<Ship>(this);
        }

        private void Actions_Dispose()
        {
            this.Actions.Dispose();
            this.Actions = null;
        }
        #endregion
    }
}
