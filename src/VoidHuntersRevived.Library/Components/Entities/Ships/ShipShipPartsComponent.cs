using Guppy.DependencyInjection;
using Guppy.Network.Components;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Entities.Ships;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Components.Entities.Ships
{
    /// <summary>
    /// Simple helper component designed to track a collection of ShipPart's
    /// within the ship's chain.
    /// </summary>
    public abstract class ShipShipPartsComponent<TShipPart> : NetworkComponent<Ship>
        where TShipPart : ShipPart
    {
        #region Private Fields
        private List<TShipPart> _shipParts;
        #endregion

        #region Public Properties
        public IEnumerable<TShipPart> ShipParts => _shipParts;
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(GuppyServiceProvider provider)
        {
            base.Initialize(provider);

            this.Entity.Chain.Root.PostTreeClean += this.HandleChainRootPostTreeClean;
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean ShouldTrack(TShipPart shipPart)
        {
            return true;
        }
        #endregion

        #region Event Handlers
        private void HandleChainRootPostTreeClean(ShipPart sender, ShipPart source, TreeComponent components)
        {
            // throw new NotImplementedException();
        }
        #endregion
    }
}
