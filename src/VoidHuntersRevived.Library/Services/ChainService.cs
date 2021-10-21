using Guppy;
using Guppy.DependencyInjection;
using Guppy.Network.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Contexts.ShipParts;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.WorldObjects;

namespace VoidHuntersRevived.Library.Services
{
    /// <summary>
    /// Simple class for creating a chain with attached
    /// ShipParts. This should more often than not be used
    /// in place of the ShipPartService, unless chainless
    /// ShipParts are specifically requested.
    /// </summary>
    public class ChainService : Service
    {
        #region Private Fields
        private GuppyServiceProvider _provider;
        private ShipPartService _shipParts;
        #endregion

        #region Initilaization Methods
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            _provider = provider;

            provider.Service(out _shipParts);
        }
        #endregion

        #region Creation Methods
        public Chain Create(ShipPart shipPart = default, Vector2 position = default, Single rotation = default)
            => _provider.GetService<Chain>(Constants.ServiceConfigurationKeys.Chain, (chain, _, _) =>
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
