using Guppy.EntityComponent;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.Network.Components;
using Guppy.Network.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;
using VoidHuntersRevived.Library.Interfaces;

namespace VoidHuntersRevived.Library.Components.WorldObjects
{
    internal class WorldObjectBaseCRUDComponent : Component<IWorldObject>
    {
        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);
        }

        protected override void PostUninitialize()
        {
            base.PostUninitialize();

            this.Entity.Pipe = default;
        }
        #endregion
    }
}
