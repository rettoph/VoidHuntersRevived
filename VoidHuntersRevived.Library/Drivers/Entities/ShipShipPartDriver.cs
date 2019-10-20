using Guppy;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities
{
    /// <summary>
    /// Simple class used to track specified ship ShipPart types
    /// and automatically update them.
    /// </summary>
    public class ShipShipPartDriver<TShipPart> : Driver<Ship>
        where TShipPart : ShipPart
    {
        #region Private Fields
        private Boolean _dirty;
        #endregion

        #region Protected Fields
        protected List<TShipPart> parts;
        #endregion

        #region Constructor
        public ShipShipPartDriver(Ship driven) : base(driven)
        {
        }
        #endregion

        #region Lifecycle Methods
        protected override void Create(IServiceProvider provider)
        {
            base.Create(provider);

            parts = new List<TShipPart>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _dirty = true;

            this.driven.Events.TryAdd<ShipPart>("bridge:chain:updated", this.HandleBridgeChainUpdated);
        }

        protected override void Dispose()
        {
            base.Dispose();

            this.parts.Clear();
        }
        #endregion

        #region Frame Methods
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_dirty)
            { // Remap all thrusters contained within the ship's components
                this.parts.Clear();
                this.parts.AddRange(this.driven.Components.Where(c => c is TShipPart).Select(c => c as TShipPart));
                _dirty = false;

                this.logger.LogDebug($"Remapped {typeof(TShipPart).Name} Parts for Ship({this.driven.Id}). Found {this.parts.Count}.");
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the bridge chain is updated, we must remap a list
        /// of all contained thrusters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleBridgeChainUpdated(object sender, ShipPart arg)
        {
            _dirty = true;
        }
        #endregion
    }
}
