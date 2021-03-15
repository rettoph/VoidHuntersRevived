using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    public abstract class ShipActionDriver<TShipPart> : Driver<Ship>
        where TShipPart : ShipPart
    {
        #region Private Fields
        private List<TShipPart> _parts;
        #endregion

        #region Protected Properties
        protected abstract UInt32 TryActionId { get; }
        protected abstract UInt32 OnActionId { get; }
        #endregion

        #region Lifecycle Methods
        protected override void Initialize(Ship driven, ServiceProvider provider)
        {
            base.Initialize(driven, provider);

            _parts = new List<TShipPart>();

            this.driven.Actions.Add(this.TryActionId, this.HandleAction);

            this.driven.OnClean += this.HandleClean;

            this.CleanParts();
        }

        protected override void Release(Ship driven)
        {
            base.Release(driven);

            this.driven.OnClean += this.HandleClean;
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean GetPartFilter(ShipPart part)
            => part is TShipPart;

        protected abstract Boolean TryAction(IEnumerable<TShipPart> parts, GameTime gameTime, params Object[] args);

        private void CleanParts()
        {
            _parts.Clear();
            _parts.TryAddRange(this.driven.Bridge?.Items(this.GetPartFilter).Select(sp => sp as TShipPart));
        }
        #endregion

        #region Event Handlers
        private void HandleAction(Ship sender, GameTime gameTime, object args)
        {
            if(this.TryAction(_parts, gameTime, args))
                this.driven.Actions.TryInvoke(this.OnActionId, gameTime, args);
        }

        private void HandleClean(Ship sender)
            => this.CleanParts();
        #endregion
    }
}
