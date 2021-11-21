using Guppy;
using Guppy.DependencyInjection;
using Guppy.Lists;
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
        protected override void PreInitialize(GuppyServiceProvider provider)
        {
            base.PreInitialize(provider);

            provider.Service(out _shipParts);
        }
        #endregion

        #region Create Methods
        public Chain Create(ShipPart shipPart = default, Vector2 position = default, Single rotation = default, Guid? id = default)
            => this.Create<Chain>(this.provider, ServiceConfigurationKeys.Chain, (chain, _, _) =>
            {
                chain.Body.Instances[NetworkAuthorization.Master].SetTransformIgnoreContacts(ref position, rotation);

                chain.Root = shipPart;
            }, id);

        public Chain Create(String contextName, Vector2 position = default, Single rotation = default, Guid? id = default)
            => this.Create(_shipParts.Create(contextName), position, rotation, id);

        public Chain Create(UInt32 contextId, Vector2 position = default, Single rotation = default, Guid? id = default)
            => this.Create(_shipParts.Create(contextId), position, rotation, id);
        #endregion

        #region GetOrCreate Methods
        public Chain GetOrCreateById(Guid id, ShipPart shipPart = default, Vector2 position = default, Single rotation = default)
                => this.GetById<Chain>(id) ?? this.Create(shipPart, position, rotation, id);

        public Chain GetOrCreateById(Guid id, String contextName, Vector2 position = default, Single rotation = default)
            => this.GetById<Chain>(id) ?? this.Create(contextName, position, rotation, id);

        public Chain GetOrCreateById(Guid id, UInt32 contextId, Vector2 position = default, Single rotation = default)
            => this.GetById<Chain>(id) ?? this.Create(contextId, position, rotation, id);
        #endregion
    }
}
