using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Interfaces;
using Guppy.Network.Attributes;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using Guppy.Network.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Enums;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Services;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    [HostTypeRequired(HostType.Remote)]
    internal class ChainBaseCRUDComponent : Component<Chain>
    {
        #region Protected Properties Fields
        protected ShipPartService shipPartService { get; private set; }
        #endregion

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.shipPartService = provider.GetService<ShipPartService>();
        }
        #endregion
    }
}
