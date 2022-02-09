using Guppy;
using Guppy.EntityComponent.DependencyInjection;
using Guppy.EntityComponent.Lists;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;
using VoidHuntersRevived.Library.Globals.Constants;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// Simple class for creating a chain with attached
    /// ShipParts. This should more often than not be used
    /// in place of the ShipPartService, unless chainless
    /// ShipParts are specifically requested.
    /// </summary>
    public class ChainService : FactoryServiceList<Chain>
    {
        #region Private Fields
        private ShipPartService _shipParts;
        #endregion

        #region Initilaization Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _shipParts);
        }
        #endregion

        #region Create Methods
        public Chain Create(ShipPart shipPart = default, Vector2 position = default, Single rotation = default)
            => this.Create<Chain>((chain, _, _) =>
            {
                chain.Body.Instances[NetworkAuthorization.Master].SetTransformIgnoreContacts(ref position, rotation);

                chain.Root = shipPart;
            });

        public Chain Create(String contextName, Vector2 position = default, Single rotation = default)
            => this.Create(_shipParts.Create(contextName), position, rotation);

        public Chain Create(UInt32 contextId, Vector2 position = default, Single rotation = default)
            => this.Create(_shipParts.Create(contextId), position, rotation);
        #endregion
    }
}
