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

            this.driven.controller.Events.TryAdd<IEnumerable<ShipPart>>("cleaned", this.ShipComponentsCleaned);
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
        }
        #endregion

        #region Event Handlers
        private void ShipComponentsCleaned(object sender, IEnumerable<ShipPart> components)
        {
            this.parts.Clear();
            this.parts.AddRange(components.Where(c => c is TShipPart).Select(c => c as TShipPart));

            this.logger.LogDebug($"Remapped {typeof(TShipPart).Name} Parts for Ship({this.driven.Id}). Found {this.parts.Count}.");
        }
        #endregion
    }
}
