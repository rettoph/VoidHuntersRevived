using Guppy;
using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Entities.Controllers;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts
{
    /// <summary>
    /// Internal driver to manage special actions when the
    /// Game's Authrization level is set to full.
    /// </summary>
    internal sealed class ShipPartFullAuthorizationNetworkDriver : NetworkEntityAuthorizationDriver<ShipPart>
    {
        #region Lifecycle Methods
        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnWrite += this.WriteMaleConnectionNode;
            this.driven.MaleConnectionNode.OnAttached += this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.MaleConnectionNode.OnDetached += this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.OnControllerChanged += this.HandleControllerChanged;
        }

        protected override void DisposeFull()
        {
            base.DisposeFull();

            this.driven.OnWrite -= this.WriteMaleConnectionNode;
            this.driven.MaleConnectionNode.OnAttached -= this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.MaleConnectionNode.OnDetached -= this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.OnControllerChanged -= this.HandleControllerChanged;
        }
        #endregion

        #region Network Methods
        public void WriteMaleConnectionNode(NetOutgoingMessage om)
        {
            om.Write("male-connection-node", m =>
            {
                if (m.WriteIf(this.driven.MaleConnectionNode.Attached))
                { // Write the male connection data, if there is any.
                    m.Write(this.driven.MaleConnectionNode.Target.Parent.Id);
                    m.Write(this.driven.MaleConnectionNode.Target.Index);
                }
            });
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// When the male connection node's attachment
        /// changes in any we we should broadcast a message 
        /// through the network
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleMaleConnectionNodeAttachmentChanged(ConnectionNode sender, ConnectionNode arg)
            => this.WriteMaleConnectionNode(this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 2));

        /// <summary>
        /// When the controller is changed we should broadcast the position just because.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void HandleControllerChanged(ShipPart sender, Controller arg)
            => BodyEntityFullAuthorizationNetworkDriver.WritePosition(this.driven, this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 8));
        #endregion
    }
}
