using Guppy;
using Guppy.DependencyInjection;
using Guppy.Extensions.System.Collections;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoidHuntersRevived.Library.Entities;
using VoidHuntersRevived.Library.Entities.ShipParts;
using VoidHuntersRevived.Library.Enums;
using Guppy.Network.Extensions.Lidgren;
using log4net;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipActionDrivers
{
    public abstract class ShipActionDriver<TShipPart> : RemoteHostDriver<Ship>
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

            this.driven.Actions.Remove(this.TryActionId, this.HandleAction);

            this.driven.OnClean -= this.HandleClean;
        }

        protected override void InitializeRemote(Ship driven, ServiceProvider provider, NetworkAuthorization networkAuthorization)
        {
            base.InitializeRemote(driven, provider, networkAuthorization);

            if(networkAuthorization == NetworkAuthorization.Master)
            { // Add the on action broadcast...
                this.driven.Actions.Add(this.OnActionId, this.Remote_HandleAction);
            }
        }

        protected override void ReleaseRemote(Ship driven, NetworkAuthorization networkAuthorization)
        {
            base.ReleaseRemote(driven, networkAuthorization);

            if(networkAuthorization == NetworkAuthorization.Master)
            { // Add the on action broadcast...
                this.driven.Actions.Remove(this.OnActionId, this.Remote_HandleAction);
            }
        }
        #endregion

        #region Helper Methods
        protected virtual Boolean GetPartFilter(ShipPart part)
            => part is TShipPart;

        protected abstract Boolean TryAction(IEnumerable<TShipPart> parts, GameTime gameTime, ref Byte data);

        private void CleanParts()
        {
            _parts.Clear();
            _parts.TryAddRange(this.driven.Bridge?.Items(this.GetPartFilter).Select(sp => sp as TShipPart));
        }
        #endregion

        #region Event Handlers
        private Boolean HandleAction(Ship sender, GameTime gameTime, Byte data)
        {
            if (this.TryAction(_parts, gameTime, ref data))
            {
                this.driven.Actions.TryInvoke(this.OnActionId, gameTime, data);
                return true;
            }

            return false;
        }

        private void HandleClean(Ship sender)
            => this.CleanParts();

        /// <summary>
        /// Broakcast the action command to the connected peer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="gameTime"></param>
        private Boolean Remote_HandleAction(Ship sender, GameTime gameTime, Byte data)
        {
            this.driven.Ping.Create(VHR.Network.MessageData.Ship.ActionPing.NetDeliveryMethod, VHR.Network.MessageData.Ship.ActionPing.SequenceChannel)
                .Write(VHR.Network.Pings.Ship.Action, om =>
                {
                    om.Write(this.TryActionId);
                    om.Write(data);
                });

            return true;
        }
        #endregion
    }
}
