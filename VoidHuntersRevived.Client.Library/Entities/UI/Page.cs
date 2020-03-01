using Guppy.UI.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities.UI
{
    public class Page : Container
    {
        #region Initialization Methods
        protected override void Initialize()
        {
            base.Initialize();

            this.Hidden = true;
        }
        #endregion
    }
}
