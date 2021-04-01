using Guppy.DependencyInjection;
using Guppy.Network.Extensions.Lidgren;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using VoidHuntersRevived.Library.Enums;

namespace VoidHuntersRevived.Library.Entities
{
    public class Team : NetworkEntity
    {
        /// <summary>
        /// The current primary color of the Ship
        /// when at full health. This is gnerally
        /// based on the team value.
        /// </summary>
        public Color Color { get; set; }

        #region Lifecycle Methods
        protected override void PreInitialize(ServiceProvider provider)
        {
            base.PreInitialize(provider);

            this.Color = Color.Cyan;

            this.MessageHandlers[MessageType.Setup].Add(this.HandleRead, this.HandleWrite);
        }

        protected override void Release()
        {
            base.Release();

            this.MessageHandlers[MessageType.Setup].Remove(this.HandleRead, this.HandleWrite);
        }
        #endregion

        #region Network Methods
        private void HandleWrite(NetOutgoingMessage om)
        {
            om.Write(this.Color);
        }

        private void HandleRead(NetIncomingMessage im)
        {
            this.Color = im.ReadColor();
        }
        #endregion
    }
}
