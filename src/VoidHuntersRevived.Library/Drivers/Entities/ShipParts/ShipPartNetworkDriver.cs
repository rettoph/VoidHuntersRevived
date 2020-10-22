using Guppy.DependencyInjection;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Entities.ShipParts;
using Guppy.Network.Extensions.Lidgren;
using VoidHuntersRevived.Library.Utilities;
using VoidHuntersRevived.Library.Entities.Controllers;
using VoidHuntersRevived.Library.Enums;
using Guppy.Extensions.DependencyInjection;
using Guppy.Lists;

namespace VoidHuntersRevived.Library.Drivers.Entities.ShipParts
{
    internal sealed class ShipPartNetworkDriver : NetworkEntityNetworkDriver<ShipPart>
    {
        #region Private Fields
        private EntityList _entities;
        #endregion

        #region Lifecycle Methods
        protected override void Configure(object driven, ServiceProvider provider)
        {
            base.Configure(driven, provider);

            this.AddAction(
                "male-connection-node", 
                this.SkipMaleConnectionNode, 
                (GameAuthorization.Minimum, this.ReadMaleConnectionNode));
        }

        protected override void Dispose()
        {
            base.Dispose();
        }

        protected override void ConfigureFull(ServiceProvider provider)
        {
            base.ConfigureFull(provider);

            this.driven.OnWrite += this.WriteMaleConnectionNode;
            this.driven.MaleConnectionNode.OnAttached += this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.MaleConnectionNode.OnDetached += this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.OnControllerChanged += this.HandleControllerChanged;
        }

        protected override void ReleaseFull()
        {
            base.ReleaseFull();

            this.driven.OnWrite -= this.WriteMaleConnectionNode;
            this.driven.MaleConnectionNode.OnAttached -= this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.MaleConnectionNode.OnDetached -= this.HandleMaleConnectionNodeAttachmentChanged;
            this.driven.OnControllerChanged -= this.HandleControllerChanged;
        }

        protected override void ConfigureMinimum(ServiceProvider provider)
        {
            base.ConfigureMinimum(provider);

            provider.Service(out _entities);
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

        private void ReadMaleConnectionNode(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
            { // Attempt to create the attachment (if any)
                this.driven.MaleConnectionNode.TryAttach(_entities.GetById<ShipPart>(im.ReadGuid()).FemaleConnectionNodes[im.ReadInt32()]);
            }
        }

        private void SkipMaleConnectionNode(NetIncomingMessage im)
        {
            if (im.ReadBoolean())
                im.Position += 160;
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
        private void HandleControllerChanged(ShipPart sender, Controller old, Controller value)
            => BodyEntityNetworkDriver.WritePosition(this.driven, this.driven.Actions.Create(NetDeliveryMethod.ReliableUnordered, 8));
        #endregion
    }
}
