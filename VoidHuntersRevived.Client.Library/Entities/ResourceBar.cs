using Guppy;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoidHuntersRevived.Client.Library.Entities
{
    /// <summary>
    /// Simple entity used to render a players resource
    /// bar either above their ship or on the bottom right
    /// of the screen (for the current user)
    /// </summary>
    public class ResourceBar : Entity
    {
        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            this.SetLayerDepth(3);
        }
        #endregion
    }
}
